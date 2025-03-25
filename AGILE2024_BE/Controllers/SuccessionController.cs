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
using AGILE2024_BE.Models.Entity.SuccessionPlan;

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
        private IHubContext<NotificationHub> hubContext;

        public SuccessionController(UserManager<ExtendedIdentityUser> um, IConfiguration co, RoleManager<IdentityRole> rm, AgileDBContext db, IHubContext<NotificationHub> hubContext)
        {
            this.userManager = um;
            this.config = co;
            this.roleManager = rm;
            this.dbContext = db;
            this.hubContext = hubContext;
        }


        [HttpGet("GetSuccessionPlans")]
        [Authorize(Roles = RolesDef.Veduci)]
        public async Task<ActionResult<IEnumerable<SuccessionPlanResponse>>> GetSuccessionPlans()
        {
            ExtendedIdentityUser? user = await userManager.FindByEmailAsync(User.Identity?.Name!);

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
                .Include(sp => sp.leavingPerson)
                    .ThenInclude(lp => lp.User)
                .Include(sp => sp.successor)
                    .ThenInclude(suc => suc.User)
                //.Where(sp => sp.leavingPerson.User.Id == user.Id)
                .ToListAsync();


            var skills = await dbContext.SuccesionSkills
            .Include(skill => skill.successionPlan) 
            .ToListAsync();


            var groupedPlans = successionPlans
                .GroupBy(sp => new { sp.leaveType.id, sp.leaveType.description })
                .Select(g => new SuccessionPlansByLeaveTypeResponse
                {
                    LeaveTypeId = g.Key.id,
                    LeaveTypeName = g.Key.description,
                    SuccessionPlans = g.Select(sp => new SuccessionPlanResponse
                    {
                        id = sp.id,
                        // Leaving employee details
                        LeavingFullName = sp.leavingPerson != null
                    ? $"{sp.leavingPerson.User.Name} {sp.leavingPerson.User.Surname}"
                    : "N/A",
                        LeavingJobPosition = sp.leavingPerson?.Level.JobPosition?.Name ?? "N/A",
                        LeavingDepartment = sp.leavingPerson?.Department?.Name ?? "N/A",
                        Reason = sp.reason ?? "N/A",
                        LeaveDate = sp.leaveDate,

                        // Successor details (can be internal/external)
                        SuccessorFullName = sp.isExternal ? "Externý zamestnanec"
                    : sp.successor != null ? $"{sp.successor.User.Name} {sp.successor.User.Surname}"
                        : "N/A",
                        SuccessorJobPosition = sp.isExternal
                    ? "N/A"
                    : sp.successor?.Level.JobPosition?.Name ?? "N/A",
                        SuccessorDepartment = sp.isExternal
                    ? "N/A"
                    : sp.successor?.Department?.Name ?? "N/A",
                        ReadyStatus = sp.readyStatus.description ?? "N/A",
                        ReadyState = CalculateReadyState(skills, sp.id.ToString())
                    }).ToList()
                }).ToList();

            return Ok(groupedPlans);
        }


        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var successionPlan = await dbContext.SuccessionPlans.FindAsync(id);

            if (successionPlan == null)
            {
                return NotFound("Record not found");
            }

            dbContext.SuccessionPlans.Remove(successionPlan);
            await dbContext.SaveChangesAsync();
            return Ok("Uspesne vymazane");
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

                if (string.IsNullOrEmpty(request.LeavingId))
                {
                    return BadRequest("Leaving person must be assigned.");
                }

                // Získanie odchádzajúceho zamestnanca (leavingPerson)
                var leavingEmployeeCard = await dbContext.EmployeeCards
                    .FirstOrDefaultAsync(ec => ec.Id.ToString() == request.LeavingId);
                if (leavingEmployeeCard == null)
                {
                    return BadRequest("Leaving person not found.");
                }

               
                var leaveType = await dbContext.LeaveTypes
                    .FirstOrDefaultAsync(g => g.id.ToString() == request.LeaveType);

                if (leaveType == null)
                {
                    return BadRequest($"Leave type {request.LeaveType} does not exist.");
                }

                var readyStatus = await dbContext.ReadyStatuses
                    .FirstOrDefaultAsync(g => g.id.ToString() == request.ReadyStatus);

                if (readyStatus == null)
                {
                    return BadRequest($"Ready status {request.LeaveType} does not exist.");
                }
                var successionPlan = new SuccessionPlan
                {
                    id = new Guid(),
                    leavingPerson = leavingEmployeeCard,
                    leaveType = leaveType,
                    reason = request.LeaveReason,
                    leaveDate = request.LeaveDate,
                    readyStatus = readyStatus,

                    isExternal = string.IsNullOrEmpty(request.SuccessorId) // Ak SuccessorId je null, nastavíme isExternal na true
                };

                if (!string.IsNullOrEmpty(request.SuccessorId))
                {
                    var successorEmployeeCard = await dbContext.EmployeeCards
                        .Include(ec => ec.User)
                        .FirstOrDefaultAsync(ec => ec.Id.ToString() == request.SuccessorId);
                    if (successorEmployeeCard != null)
                    {
                        successionPlan.successor = successorEmployeeCard;
                    }
                    else
                    {
                        return BadRequest("Successor not found.");
                    }
                }

                if (request.Skills != null && request.Skills.Any())
                {
                    var skills = request.Skills.Select(skillRequest => new SuccesionSkills
                    {
                        description = skillRequest.Description,
                        isReady = skillRequest.IsReady,
                        successionPlan = successionPlan 
                    }).ToList();

                    dbContext.SuccesionSkills.AddRange(skills);
                }

                dbContext.SuccessionPlans.Add(successionPlan);

                if (request.isNotified && successionPlan.successor != null) {
                  

                    var notification = new Notification
                {
                    Id = Guid.NewGuid(),
                    User = successionPlan.successor.User,
                    ReferencedItemId = successionPlan.id,
                    Message = $"Bolo Vám pridelené nástupníctvo na novú pracovnú pozíciu!",
                    CreatedAt = DateTime.UtcNow,
                    IsRead = false,
                    NotificationType = EnumNotificationType.NewSuccessionNotificationType
                };

                dbContext.Notifications.Add(notification);
                await dbContext.SaveChangesAsync();

                await hubContext.Clients.User(notification.User.Id)
                            .SendAsync("ReceiveNotification", new NotificationResponse
                            {
                                Id = notification.Id,
                                Message = notification.Message,
                                Title = NotificationHelpers.GetNotificationTitle(notification.NotificationType),
                                ReferencedItem = notification.ReferencedItemId.ToString(),
                                NotificationType = notification.NotificationType,
                                CreatedAt = notification.CreatedAt,
                                IsRead = notification.IsRead
                            });
                }

                return Ok(new { message = "Succession plan created successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while creating the goal.");
            }
        }

        [HttpGet("GetById/{id}")]
        [Authorize(Roles = RolesDef.Veduci)]
        public async Task<ActionResult<SuccessionPlanEditResponse>> GetById(Guid id)
        {
            var successionPlan = await dbContext.SuccessionPlans
                .Include(sp => sp.leaveType)
                .Include(sp => sp.readyStatus)
                .Include(sp => sp.successor)
                    .ThenInclude(ec => ec.Level.JobPosition)
                .Include(sp => sp.successor)
                    .ThenInclude(ec => ec.Department)
                    .ThenInclude(d => d.Organization)
                .Include(sp => sp.leavingPerson)
                    .ThenInclude(lp => lp.Level.JobPosition)
                .Include(sp => sp.leavingPerson)
                    .ThenInclude(lp => lp.Department)
                .Include(sp => sp.leavingPerson)
                    .ThenInclude(lp => lp.User)
                .Include(sp => sp.successor)
                    .ThenInclude(suc => suc.User)
                .FirstOrDefaultAsync(sp => sp.id == id);

            if (successionPlan == null)
            {
                return NotFound("Succession plan not found.");
            }

            var skills = await dbContext.SuccesionSkills
                .Where(s => s.successionPlan.id == id)
                .Select(skill => new SuccessionSkillResponse
                {
                    Description = skill.description,
                    IsReady = skill.isReady
                }).ToListAsync();

            var response = new SuccessionPlanEditResponse
            {
                id = successionPlan.id,
                LeaveType = successionPlan.leaveType.description,
                LeavingId = successionPlan.leavingPerson.Id,
                LeavingFullName = successionPlan.leavingPerson != null
                    ? $"{successionPlan.leavingPerson.User.Name} {successionPlan.leavingPerson.User.Surname}"
                    : "N/A",
                LeavingJobPosition = successionPlan.leavingPerson?.Level.JobPosition?.Name ?? "N/A",
                LeavingDepartment = successionPlan.leavingPerson?.Department?.Name ?? "N/A",
                LeavingOrganization = successionPlan.leavingPerson?.Department?.Organization?.Name ?? "N/A",
                Reason = successionPlan.reason ?? "N/A",
                LeaveDate = successionPlan.leaveDate,
                SuccessorId = successionPlan.successor?.Id.ToString(),
                SuccessorFullName = successionPlan.isExternal
                    ? "N/A"
                    : successionPlan.successor != null
                        ? $"{successionPlan.successor.User.Name} {successionPlan.successor.User.Surname}"
                        : "N/A",
                SuccessorJobPosition = successionPlan.isExternal
                    ? "N/A"
                    : successionPlan.successor?.Level.JobPosition?.Name ?? "N/A",
                SuccessorDepartment = successionPlan.isExternal
                    ? "N/A"
                    : successionPlan.successor?.Department?.Name ?? "N/A",
                ReadyStatusId = successionPlan.readyStatus.id.ToString(),
                ReadyStatus = successionPlan.readyStatus?.description ?? "N/A",
                Skills = skills
            };

            return Ok(response);
        }

        [HttpPut("Update/{id}")]
        [Authorize(Roles = RolesDef.Veduci)]
        public async Task<IActionResult> UpdateSuccession(Guid id, [FromBody] UpdateSuccessionRequest request)
        {
            try
            {
                var successionPlan = await dbContext.SuccessionPlans
                    .Include(sp => sp.leaveType)
                    .Include(sp => sp.readyStatus)
                    .Include(sp => sp.successor)
                    .Include(sp => sp.leavingPerson)
                    .FirstOrDefaultAsync(sp => sp.id == id);

                if (successionPlan == null)
                {
                    return NotFound("Succession plan not found.");
                }

                var readyStatus = await dbContext.ReadyStatuses
                    .FirstOrDefaultAsync(g => g.id.ToString() == request.ReadyStatusId);
                if (readyStatus == null)
                {
                    return BadRequest($"Ready status {request.ReadyStatus} does not exist.");
                }

                successionPlan.readyStatus = readyStatus;
                successionPlan.isExternal = string.IsNullOrEmpty(request.SuccessorId);

                if (!successionPlan.isExternal)
                {
                    var successorEmployeeCard = await dbContext.EmployeeCards
                        .FirstOrDefaultAsync(ec => ec.Id.ToString() == request.SuccessorId);
                    if (successorEmployeeCard == null)
                    {
                        return BadRequest("Successor not found.");
                    }

                    successionPlan.successor = successorEmployeeCard;
                }

                // Handle SuccesionSkills manually
                var existingSkills = await dbContext.SuccesionSkills
                    .Where(skill => skill.successionPlan.id == id)
                    .ToListAsync();

                dbContext.SuccesionSkills.RemoveRange(existingSkills);

                if (request.Skills != null && request.Skills.Any())
                {
                    var newSkills = request.Skills.Select(skillRequest => new SuccesionSkills
                    {
                        successionPlan = successionPlan, // Set the foreign key relationship
                        description = skillRequest.Description,
                        isReady = skillRequest.IsReady
                    });

                    await dbContext.SuccesionSkills.AddRangeAsync(newSkills);
                }

                await dbContext.SaveChangesAsync();
                               
                return Ok(new { message = "Succession plan updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        private string CalculateReadyState(List<SuccesionSkills> skills, string successionPlanId)
        {
            var planSkills = skills.Where(skill => skill.successionPlan.id.ToString() == successionPlanId).ToList();
            if (!planSkills.Any())
                return "0%";

            int totalSkills = planSkills.Count;
            int readySkills = planSkills.Count(skill => skill.isReady == true);

            int readyPercentage = (int)((double)readySkills / totalSkills * 100);

            return $"{readyPercentage}%"; 
        }

    }

    public class UpdateSuccessionRequest
    {

        public string? SuccessorId { get; set; }
        public string? ReadyStatus { get; set; }
        public string? ReadyStatusId { get; set; }
        public List<SkillRequest>? Skills { get; set; }

        public bool isNotified { get; set; }
    }

    public class SkillRequest
    {
        public string Description { get; set; }
        public bool IsReady { get; set; }
    }

}
