﻿using AGILE2024_BE.Data;
using AGILE2024_BE.Models;
using AGILE2024_BE.Models.Identity;
using AGILE2024_BE.Models.Requests.GoalRequests;
using AGILE2024_BE.Models.Response;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AGILE2024_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GoalController : ControllerBase
    {
        private UserManager<ExtendedIdentityUser> userManager;
        private RoleManager<IdentityRole> roleManager;
        private IConfiguration config;
        private AgileDBContext dbContext;

        public GoalController(UserManager<ExtendedIdentityUser> um, IConfiguration co,  RoleManager<IdentityRole> rm, AgileDBContext db)
        {
            this.userManager = um;
            this.config = co;
            this.roleManager = rm;
            this.dbContext = db;
        }

        [HttpGet("Goals")]
        [Authorize(Roles = RolesDef.Veduci)]
        public async Task<IActionResult> Goals()
        {
            try
            {
                ExtendedIdentityUser? user = await userManager.FindByEmailAsync(User.Identity?.Name!);

                /*var goals1 = await dbContext.Goals
                    .Include(g => g.employee)
                    .ThenInclude(e => e.User)
                    .Where(g => g.employee.User.Id == user.Id).ToListAsync();*/

                var goals = await dbContext.Goals
                     .Include(g => g.employee)
                    .ThenInclude(e => e.User)
                    .Include(g => g.category)
                    .Include(g => g.status)
                    .Where(g => g.employee.User.Id == user.Id) 
                    .Select(g => new
                    {
                        g.id,
                        g.name,
                        g.description,
                        g.finishedDate,
                        g.fullfilmentRate,
                        g.dueDate,

                        categoryDescription = g.category.description,
                        statusDescription = g.status.description,
                        AssignedEmployees = dbContext.GoalAssignments
                        .Include(g => g.employee)
                        .ThenInclude(e => e.User)
                        .Include(g => g.goal)
                        .Where(ga => ga.goal.id == g.id)
                        .Select(ga => new
                    {
                        ga.employee.Id,
                        ga.employee.User.Name,
                        ga.employee.User.Surname
                    })
                    .ToList()
                    })
                    .ToListAsync();

                if (!goals.Any())
                {
                    return NoContent();
                }

                return Ok(goals);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Došlo k internej chybe pri načítavaní cieľov.");
            }
        }

        [HttpPost("Create")]
        [Authorize(Roles = RolesDef.Veduci)]
        public async Task<IActionResult> CreateGoal([FromBody] CreateGoalRequest request)
        {
            try
            {
                ExtendedIdentityUser? user = await userManager.FindByEmailAsync(User.Identity?.Name!);
                if (user == null)
                {
                    return Unauthorized("User not found.");
                }

                if (request.EmployeeIds == null || !request.EmployeeIds.Any())
                {
                    return BadRequest("At least one employee must be assigned to the goal.");
                }

                var employeeCard = await dbContext.EmployeeCards
                    .FirstOrDefaultAsync(ec => ec.User.Id == user.Id);
                if (employeeCard == null)
                {
                    return Unauthorized("Employee card not found for the logged-in user.");
                }
                var goalCategory = await dbContext.GoalCategory
                    .FirstOrDefaultAsync(g => g.id.ToString() == request.GoalCategoryId);

                if (goalCategory == null)
                {
                    return BadRequest($"Goal category {request.GoalCategoryId} does not exist.");
                }

                var goalStatus = await dbContext.GoalStatuses
                .FirstOrDefaultAsync(s => s.description == "Nezačatý");

                if (goalStatus == null)
                {
                    return BadRequest("Status 'Nezačatý' does not exist.");
                }
                var newGoal = new Goal
                {
                    name = request.Name,
                    description = request.Description,
                    category = goalCategory,
                    status = goalStatus,
                    dueDate = DateTime.Parse(request.DueDate), 
                    employee = employeeCard
                };
                dbContext.Goals.Add(newGoal);
                await dbContext.SaveChangesAsync();
                if (request.EmployeeIds != null && request.EmployeeIds.Any())
                {
                    foreach (var employeeId in request.EmployeeIds)
                    {
                        var employee = await dbContext.EmployeeCards
                            .Include(e => e.User)
                            .FirstOrDefaultAsync(e => e.Id.ToString() == employeeId);

                        if (employee != null)
                        {
                            var goalAssignment = new GoalAssignment
                            {
                                goal = newGoal,
                                employee = employee
                            };

                            dbContext.GoalAssignments.Add(goalAssignment);
                        }
                    }
                    await dbContext.SaveChangesAsync();
                }

                return Ok(new { message = "Goal created successfully.", goalId = newGoal.id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while creating the goal.");
            }
        }

        [HttpPut("Edit/{id}")]
        [Authorize(Roles = RolesDef.Veduci)]
        public async Task<IActionResult> EditGoal(Guid id, [FromBody] EditGoalRequest request)
        {
            try
            {
                ExtendedIdentityUser? user = await userManager.FindByEmailAsync(User.Identity?.Name!);

                if (user == null)
                {
                    return Unauthorized("User not found.");
                }

                var employeeCard = await dbContext.EmployeeCards
                    .FirstOrDefaultAsync(ec => ec.User.Id == user.Id);

                if (employeeCard == null)
                {
                    return Unauthorized("Employee card not found for the logged-in user.");
                }


                var goal = await dbContext.Goals
                    .Include(g => g.category)
                    .Include(g => g.status)
                    .Include(g => g.employee) 
                    .FirstOrDefaultAsync(g => g.id == id);

                if (goal == null)
                {
                    return NotFound($"Goal with ID {id} not found.");
                }

                var goalCategory = await dbContext.GoalCategory
                    .FirstOrDefaultAsync(g => g.id.ToString() == request.GoalCategoryId);

                if (goalCategory == null)
                {
                    return BadRequest($"Goal category {request.GoalCategoryId} does not exist.");
                }

                var goalStatus = await dbContext.GoalStatuses

                    .FirstOrDefaultAsync(s => s.id == request.StatusId);

                if (goalStatus == null)
                {
                    return BadRequest($"Goal status {request.StatusId} does not exist.");

                }

                goal.name = request.Name;
                goal.description = request.Description;
                goal.category = goalCategory;
                goal.status = goalStatus; 
                goal.dueDate = request.DueDate;

                
                if (request.FullfilmentRate != null)
                {
                    goal.fullfilmentRate = request.FullfilmentRate;
                }

                if (request.FinishedDate != null)
                {
                    goal.finishedDate = request.FinishedDate;
                }

                // Update the goal in the database
                dbContext.Goals.Update(goal);
                await dbContext.SaveChangesAsync();

                // Remove all existing assignments for the goal before adding new ones
                var existingAssignments = dbContext.GoalAssignments
                    .Where(ga => ga.goal.id == goal.id);

                dbContext.GoalAssignments.RemoveRange(existingAssignments);
                await dbContext.SaveChangesAsync();

                // Reassign the new employees if employee IDs are provided
                if (request.EmployeeIds != null && request.EmployeeIds.Any())
                {
                    foreach (var employeeId in request.EmployeeIds)
                    {
                        var employee = await dbContext.EmployeeCards
                            .Include(e => e.User)
                            .FirstOrDefaultAsync(e => e.Id.ToString() == employeeId);

                        if (employee != null)
                        {
                            var goalAssignment = new GoalAssignment
                            {
                                goal = goal,
                                employee = employee
                            };

                            dbContext.GoalAssignments.Add(goalAssignment);
                        }
                    }
                    await dbContext.SaveChangesAsync();
                }

                return Ok(new { message = "Goal updated successfully.", goalId = goal.id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while updating the goal.");
            }
        }

        [HttpPut("EditEmployee/{id}")]
        [Authorize(Roles = RolesDef.Zamestnanec)]
        public async Task<IActionResult> EditEmployeeGoal(Guid id, [FromBody] EditEmployeeGoalRequest request)
        {
            try
            {
                /*ExtendedIdentityUser? user = await userManager.FindByEmailAsync(User.Identity?.Name!);

                if (user == null)
                {
                    return Unauthorized("User not found.");
                }

                var employeeCard = await dbContext.EmployeeCards
                    .FirstOrDefaultAsync(ec => ec.User.Id == user.Id);

                if (employeeCard == null)
                {
                    return Unauthorized("Employee card not found for the logged-in user.");
                }*/

                var goal = await dbContext.Goals
                    .Include(g => g.category)
                    .Include(g => g.status)
                    .Include(g => g.employee)
                    .FirstOrDefaultAsync(g => g.id == id);

                if (goal == null)
                {
                    return NotFound($"Goal with ID {id} not found.");
                }

               

                var goalStatus = await dbContext.GoalStatuses
                    .FirstOrDefaultAsync(s => s.id.ToString() == request.Status);

                if (goalStatus == null)
                {
                    return BadRequest($"Goal status {request.Status} does not exist.");
                }

                goal.status = goalStatus;


                if (request.FullfilmentRate != null)
                {
                    goal.fullfilmentRate = request.FullfilmentRate;
                }
                else { goal.fullfilmentRate = null; }

                if (request.FinishedDate != null)
                {
                    goal.finishedDate = request.FinishedDate;
                }
                else { goal.finishedDate = null; }

                dbContext.Goals.Update(goal);
                await dbContext.SaveChangesAsync();

           
                return Ok(new { message = "Goal updated successfully.", goalId = goal.id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while updating the goal.");
            }
        }


        [HttpDelete("Delete/{goalId}")]
        [Authorize(Roles = RolesDef.Veduci)]
        public async Task<IActionResult> DeleteGoal(Guid goalId)
        {
            try
            {
                var goal = await dbContext.Goals
                    .FirstOrDefaultAsync(g => g.id == goalId);

                if (goal == null)
                {
                    return NotFound($"Goal with ID {goalId} not found.");
                }

                var goalAssignments = await dbContext.GoalAssignments
                    .Where(ga => ga.goal == goal)
                    .ToListAsync();

                dbContext.GoalAssignments.RemoveRange(goalAssignments);

                dbContext.Goals.Remove(goal);

                await dbContext.SaveChangesAsync();

                return Ok(new { message = "Goal deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while deleting the goal.");
            }
        }

        [HttpGet("GoalEmployees/{goalId}")]
        [Authorize(Roles = RolesDef.Veduci)]
        public async Task<IActionResult> GetEmployeesForGoal(Guid goalId)
        {
            try
            {
                var goal = await dbContext.Goals
                    .FirstOrDefaultAsync(g => g.id == goalId);
                if (goal == null)
                {
                    return NotFound($"Goal with ID {goalId} not found.");
                }
                var goalAssignments = await dbContext.GoalAssignments
                    .Include(ga => ga.employee)   
                    .ThenInclude(e => e.User)    
                    .Where(ga => ga.goal.id == goalId)
                    .Select(ga => new
                    {
                        ga.employee.Id,
                        ga.employee.User.Name,
                        ga.employee.User.Surname,
                        ga.employee.User.Email 
                    })
                    .ToListAsync();
                if (!goalAssignments.Any())
                {
                    return NotFound("No employees are assigned to this goal.");
                }
                return Ok(goalAssignments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while retrieving the employees.");
            }
        }

        [HttpGet("MyGoals")]
        [Authorize(Roles = RolesDef.Zamestnanec)]
        public async Task<IActionResult> MyGoals()
        {
            try
            {
                ExtendedIdentityUser? user = await userManager.FindByEmailAsync(User.Identity?.Name!);


                var loggedEmployee = await dbContext.EmployeeCards
                    .Include(ec => ec.User)
                    .Where(ec => ec.User.Id == user.Id).FirstOrDefaultAsync();

                if (loggedEmployee == null)
                    return NotFound("Zamestnanec nenájdený.");

                var goalsAssigned = await dbContext.GoalAssignments
                    .Include(ga => ga.employee)
                    .Include(ga => ga.goal)
                    .Where(ga => ga.employee.Id == loggedEmployee.Id)
                    .ToListAsync();

                var goalIds = goalsAssigned.Select(ga => ga.goal.id).ToList();
                var goals = await dbContext.Goals
                .Where(g => goalIds.Contains(g.id)) // Výber priradených cieľov
                .Include(g => g.category) // Zahrnutie kategórie
                .Include(g => g.status) // Zahrnutie stavu
                .Select(g => new
                {
                    g.id,
                    g.name,
                    g.description,
                    g.finishedDate,
                    g.fullfilmentRate,
                    g.dueDate,
                    categoryDescription = g.category.description,
                    statusDescription = g.status.description
                
                })
                .ToListAsync();

                // Ak nie sú žiadne ciele, vráťte NoContent
                if (!goals.Any())
                    return NoContent();

                return Ok(goals);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Došlo k internej chybe pri načítavaní cieľov.");
            }
        }

        [HttpGet("{goalId}")]
        [Authorize]
        public async Task<IActionResult> GetGoalById(Guid goalId)
        {
            var goal = await dbContext.Goals
        .Include(d => d.employee)
        .Include(d => d.status) 
        .Include(d => d.category)
        .FirstOrDefaultAsync(d => d.id == goalId);

            if (goal == null)
            {
                return NotFound(new { message = "Oddelenie s týmto ID neexistuje." });
            }

            var departmentResponse = new GoalResponse
            {
                Id = goal.id,
                Name = goal.name,
                Description = goal.description,
                GoalCategoryId = goal.category.id.ToString(),
                GoalCategoryName = goal.category.description,
                StatusId = goal.status.id,
                StatusName = goal.status?.description,
                DueDate = goal.dueDate,
                FinishedDate = goal.finishedDate,
                FullfilmentRate = goal.fullfilmentRate,
            };

            return Ok(departmentResponse);
        }
    }
}
