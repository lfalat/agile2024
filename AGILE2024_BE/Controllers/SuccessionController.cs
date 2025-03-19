using AGILE2024_BE.Models.Requests;
using AGILE2024_BE.Models.Response;
using AGILE2024_BE.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using AGILE2024_BE.Data;
using AGILE2024_BE.Models.Identity;
using Microsoft.EntityFrameworkCore;
using AGILE2024_BE.Models;
using Azure.Core;
using AGILE2024_BE.Models.Requests.GoalRequests;
using AGILE2024_BE.Services;
using Microsoft.AspNetCore.SignalR;

namespace AGILE2024_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SuccessionController : ControllerBase
    {
        private UserManager<ExtendedIdentityUser> userManager;
        private RoleManager<IdentityRole> roleManager;
        private IConfiguration config;
        private AgileDBContext dbContext;

        public SuccessionController(UserManager<ExtendedIdentityUser> um, IConfiguration co, RoleManager<IdentityRole> rm, AgileDBContext db)
        {
            this.userManager = um;
            this.config = co;
            this.roleManager = rm;
            this.dbContext = db;
        }


        [HttpGet("GetSuccessionPlans")]
        public async Task<ActionResult<IEnumerable<SuccessionPlanResponse>>> GetSuccessionPlans()
        {
            var successionPlans = await dbContext.SuccessionPlans
                .Include(sp => sp.leaveType)
                .Include(sp => sp.readyStatus)
                .Include(sp => sp.successor) // Successor
                    .ThenInclude(ec => ec.Level.JobPosition)
                .Include(sp => sp.successor) // Successor
                    .ThenInclude(ec => ec.Department)
                .Include(sp => sp.leavingPerson) // Leaving employee
                    .ThenInclude(lp => lp.Level.JobPosition)
                .Include(sp => sp.leavingPerson) // Leaving employee
                    .ThenInclude(lp => lp.Department)
                .ToListAsync();


            var groupedPlans = successionPlans
                .GroupBy(sp => new { sp.leaveType.id, sp.leaveType.description })
                .Select(g => new SuccessionPlansByLeaveTypeResponse
                {
                    LeaveTypeId = g.Key.id,
                    LeaveTypeName = g.Key.description,
                    SuccessionPlans = g.Select(sp => new SuccessionPlanResponse
                    {
                        // Leaving employee details
                        LeavingFullName = sp.leavingPerson != null
                    ? $"{sp.leavingPerson.User.Name} {sp.leavingPerson.User.Surname}"
                    : "N/A",
                        LeavingJobPosition = sp.leavingPerson?.Level.JobPosition?.Name ?? "N/A",
                        LeavingDepartment = sp.leavingPerson?.Department?.Name ?? "N/A",
                        Reason = sp.reason ?? "N/A",
                        LeaveDate = sp.leaveDate,

                        // Successor details (can be internal/external)
                        SuccessorFullName = sp.isExternal ? "N/A"
                    : sp.successor != null ? $"{sp.successor.User.Name} {sp.successor.User.Surname}"
                        : "N/A",
                        SuccessorJobPosition = sp.isExternal
                    ? "N/A"
                    : sp.successor?.Level.JobPosition?.Name ?? "N/A",
                        SuccessorDepartment = sp.isExternal
                    ? "N/A"
                    : sp.successor?.Department?.Name ?? "N/A",
                        ReadyStatus = sp.readyStatus.description ?? "N/A"
                    }).ToList()
                }).ToList();

            return Ok(groupedPlans);
        }

        [HttpGet("GetLeaveTypes")]
        public async Task<ActionResult> GetLeaveTypes()
        {
            var leaveTypes = await dbContext.LeaveTypes
               .Select(l => new 
               {
                   Id = l.id,
                   Description = l.description
               })
               .ToListAsync();

            return Ok(leaveTypes);
        }

        [HttpGet("GetReadyStatuses")]
        public async Task<ActionResult> GetReadyStatuses()
        {
            var readyStatuses = await dbContext.ReadyStatuses
               .Select(l => new
               {
                   Id = l.id,
                   Description = l.description
               })
               .ToListAsync();

            return Ok(readyStatuses);
        }

        [HttpPost("Create")]
        [Authorize(Roles = RolesDef.Veduci)]
        public async Task<IActionResult> CreateSuccession([FromBody] CreateSuccessionRequest request)
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

                List<Notification> notifications = new List<Notification>();

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

                            var notification = new Notification
                            {
                                Id = Guid.NewGuid(),
                                User = employee.User,
                                ReferencedItemId = newGoal.id,
                                Message = $"Bol Vám priradený nový cieľ '{newGoal.name}'. Prosím, pre bližšie informácie si pozrite detail cieľa !",
                                CreatedAt = DateTime.UtcNow,
                                IsRead = false,
                                NotificationType = EnumNotificationType.GoalCreatedNotificationType
                            };

                            notifications.Add(notification);
                        }
                    }
                    dbContext.Notifications.AddRange(notifications);
                    await dbContext.SaveChangesAsync();

                    foreach (var notification in notifications)
                    {
                        await hubContext.Clients.User(notification.User.Id)
                            .SendAsync("ReceiveNotification", new NotificationResponse
                            {
                                Id = notification.Id,
                                Message = notification.Message,
                                Title = NotificationHelpers.GetNotificationTitle(notification.NotificationType),
                                //Link = $"/goals/{notification.ReferencedItemId}",
                                ReferencedItem = notification.ReferencedItemId.ToString(),
                                NotificationType = notification.NotificationType,
                                CreatedAt = notification.CreatedAt,
                                IsRead = notification.IsRead
                            });
                    }
                }

                return Ok(new { message = "Goal created successfully.", goalId = newGoal.id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while creating the goal.");
            }
        }

    }
}
