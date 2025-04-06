using AGILE2024_BE.Data;
using AGILE2024_BE.Models;
using AGILE2024_BE.Models.Identity;
using AGILE2024_BE.Models.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;
using AGILE2024_BE.Models.Enums;
using Microsoft.EntityFrameworkCore;
using AGILE2024_BE.Models.Response.Feedback;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using AGILE2024_BE.Models.Entity.Adaptation;
using Azure.Core;
namespace AGILE2024_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AdaptationController : ControllerBase
    {
        private UserManager<ExtendedIdentityUser> userManager;
        private RoleManager<IdentityRole> roleManager;
        private IConfiguration config;
        private AgileDBContext dbContext;

        public AdaptationController(UserManager<ExtendedIdentityUser> um, IConfiguration co, RoleManager<IdentityRole> rm, AgileDBContext db)
        {
            this.userManager = um;
            this.config = co;
            this.roleManager = rm;
            this.dbContext = db;
        }

        [HttpGet("GetAdaptations")]
        [Authorize(Roles = RolesDef.Veduci)]
        public async Task<IActionResult> Adaptations()
        {
            try
            {
                ExtendedIdentityUser? user = await userManager.FindByEmailAsync(User.Identity?.Name!);

                var adaptations = await dbContext.Adaptations
                     .Include(a => a.Employee)
                         .ThenInclude(e => e.User)
                     .Include(a => a.CreatedEmployee)
                         .ThenInclude(e => e.User)
                     .Include(a => a.State)
                     .Select(a => new AdaptationResponse
                     {
                         id = a.Id,
                         nameEmployee = $"{a.Employee.User.Name} {a.Employee.User.Surname}",
                         nameSupervisor = $"{a.CreatedEmployee.User.Name} {a.CreatedEmployee.User.Surname}",
                         readyDate = a.ReadyDate,
                         endDate = a.EndDate,
                         taskState = a.State.DescriptionState
                     })
                     .ToListAsync();
                if (!adaptations.Any())
                    return NoContent();

                return Ok(adaptations);
            } 
            catch (Exception ex)
            {
                return StatusCode(500, "Chyba pri nacitavani.");
            }
        }



        [HttpGet("GetMineAdaptations")]
        [Authorize(Roles = RolesDef.Zamestnanec)]
        public async Task<IActionResult> MineAdaptations()
        {
            try
            {
                ExtendedIdentityUser? user = await userManager.FindByEmailAsync(User.Identity?.Name!);

                var loggedEmployee = await dbContext.EmployeeCards
                    .Include(ec => ec.User)
                    .Where(ec => ec.User.Id == user.Id).FirstOrDefaultAsync();

                if (loggedEmployee == null)
                    return NotFound("Zamestnanec nenájdený.");


                var adaptations = await dbContext.Adaptations
                     .Include(a => a.Employee)
                         .ThenInclude(e => e.User)
                         .Where(e => e.Employee.Id == loggedEmployee.Id)
                     .Include(a => a.CreatedEmployee)
                         .ThenInclude(e => e.User)
                     .Include(a => a.State)
                     .Select(a => new AdaptationResponse
                     {
                         id = a.Id,
                         nameEmployee = $"{a.Employee.User.Name} {a.Employee.User.Surname}",
                         nameSupervisor = $"{a.CreatedEmployee.User.Name} {a.CreatedEmployee.User.Surname}",
                         readyDate = a.ReadyDate,
                         endDate = a.EndDate,
                         taskState = a.State.DescriptionState
                     })
                     .ToListAsync();
                if (!adaptations.Any())
                    return NoContent();

                return Ok(adaptations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Chyba pri nacitavani.");
            }
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CreateAdaptation([FromBody] CreateAdaptationRequest data)
        {
            var adaptationId = Guid.NewGuid();

            var employeerUser = await dbContext.Users
               .FirstOrDefaultAsync(u => u.Id == data.EmployeeId.ToString());

            var employeeEmployeeCard = await dbContext.EmployeeCards
                .FirstOrDefaultAsync(ec => ec.User.Id == employeerUser.Id);


            var createUser = await dbContext.Users
             .FirstOrDefaultAsync(u => u.Id == data.CreatedEmployeeId.ToString());

            var createEmployeeCard = await dbContext.EmployeeCards
                .FirstOrDefaultAsync(ec => ec.User.Id == createUser.Id);


            var state = await dbContext.AdaptationStates
                .FirstOrDefaultAsync(s => s.Id == data.StateId);

            var adaptation = new Adaptation
            {
                Id = adaptationId,
                Employee = employeeEmployeeCard,
                CreatedEmployee = createEmployeeCard,
                ReadyDate = data.Tasks.Max(t => t.FinishDate),
                EndDate = data.Tasks.All(t => t.IsDone) ? DateOnly.FromDateTime(DateTime.Now) : default,
                State = state,
            };

            dbContext.Adaptations.Add(adaptation);
            await dbContext.SaveChangesAsync();
          
            var taskEntities = data.Tasks.Select(t => new AdaptationTask
            {
                Id = Guid.NewGuid(),
                Adaptation = adaptation,
                DescriptionTask = t.Description,
                IsDone = t.IsDone,
                FinishDate = t.FinishDate.ToDateTime(TimeOnly.MinValue)
            }).ToList();

            var docEntities = data.Documents.Select(d => new AdaptationDoc
            {
                Id = Guid.NewGuid(),
                Adaptation = adaptation,
                DescriptionDocs = d.Description,
                FilePath = d.FilePath
            }).ToList();

            
            await dbContext.AdaptationTasks.AddRangeAsync(taskEntities);
            await dbContext.AdaptationDocs.AddRangeAsync(docEntities);
            await dbContext.SaveChangesAsync();

            return Ok(new { adaptationId });
        }
    }


    public class CreateAdaptationRequest
    {
        public Guid EmployeeId { get; set; }
        public Guid CreatedEmployeeId { get; set; }
        public Guid StateId { get; set; }  // e.g., some default state like 'InProgress'

        public List<AdaptationTaskRequest> Tasks { get; set; }
        public List<AdaptationDocRequest> Documents { get; set; }
    }

    public class AdaptationTaskRequest
    {
        public string Description { get; set; }
        public DateOnly FinishDate { get; set; }
        public bool IsDone { get; set; }
    }

    public class AdaptationDocRequest
    {
        public string Description { get; set; }
        public string FilePath { get; set; }
    }

}
