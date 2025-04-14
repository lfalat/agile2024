using AGILE2024_BE.Data;
using AGILE2024_BE.Models.Entity.Courses;
using AGILE2024_BE.Models.Identity;
using AGILE2024_BE.Models.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;


namespace AGILE2024_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly IHubContext<NotificationHub> hubContext;
        private UserManager<ExtendedIdentityUser> userManager;
        private RoleManager<IdentityRole> roleManager;
        private IConfiguration config;
        private AgileDBContext dbContext;

        public CoursesController(UserManager<ExtendedIdentityUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration config, AgileDBContext dbContext, IHubContext<NotificationHub> hubContext)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.config = config;
            this.dbContext = dbContext;
            this.hubContext = hubContext;
        }

        [HttpGet("Get/{selectedUserId}")]
        public async Task<ActionResult<List<CourseEmployeeResponse>>> GetCourseEmployees(string selectedUserId)
        {
            var courseEmployees = await dbContext.CourseEmployees
                .Include(ce => ce.Course)
                    .ThenInclude(c => c.Type)
                .Include(ce => ce.Course)
                    .ThenInclude(c => c.CreatedEmployee)
                        .ThenInclude(e => e.Department)
                .Include(ce => ce.Employee)
                    .ThenInclude(e => e.User)
                .Include(ce => ce.State)
                .Where(ce => ce.Employee.User.Id == selectedUserId)
                .ToListAsync();

            var all = await dbContext.CourseEmployees
                .Include(x => x.Course)
                .Include(x => x.Employee)
                    .ThenInclude(xy => xy.User)
                .ToListAsync();

            var result = courseEmployees.Select(ce => new CourseEmployeeResponse
            {
                Id = ce.Id,
                CourseName = ce.Course.Name,
                ExpirationDate = ce.Course.ExpirationDate,
                CourseType = ce.Course.Type.DescriptionType,
                Version = ce.Course.Version,
                DetailDescription = ce.Course.DetailDescription,
                EmployeeId = ce.Employee.User.Id,
                EmployeeName = ce.Employee.User.Name,
                CourseState = ce.State.DescriptionState,
                CompletedDate = ce.CompletedDate,
                CreatedBy = ce.Course.CreatedEmployee.User.Name + " " + ce.Course.CreatedEmployee.User.Surname,
                Department = ce.Course.CreatedEmployee.Department.Name
            }).ToList();

            return Ok(result);
        }


        [HttpDelete("DeleteCourseEmployee/{id}")]
        [Authorize(Roles = RolesDef.Veduci)]
        public async Task<IActionResult> DeleteCourseEmployee(Guid id)
        {
            var courseEmployee = await dbContext.CourseEmployees.FindAsync(id);

            if (courseEmployee == null)
            {
                return NotFound(new { message = "CourseEmployee not found." });
            }

            dbContext.CourseEmployees.Remove(courseEmployee);
            await dbContext.SaveChangesAsync();

            return Ok(new { message = "Deleted successfully." });
        }


        [HttpGet("getAll")]
        public async Task<ActionResult<List<CourseEmployeeResponse>>> GetAllCourseEmployees()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            bool isVeduci = role == "Vedúci zamestnanec";

            if (!Guid.TryParse(userId, out Guid userGuid))
            {
                return BadRequest("Invalid user ID");
            }

            if (!isVeduci)
            {
                return await GetCourseEmployees(userId);
            }
            try
            {
                var courseEmployeesQuery = dbContext.CourseEmployees
                    .Include(ce => ce.Course)
                        .ThenInclude(c => c.Type)
                    .Include(ce => ce.Course)
                        .ThenInclude(c => c.CreatedEmployee)
                            .ThenInclude(e => e.User)
                    .Include(ce => ce.Course)
                        .ThenInclude(c => c.CreatedEmployee)
                            .ThenInclude(c => c.Department)
                    .Include(ce => ce.Employee)
                        .ThenInclude(e => e.User)
                    .Include(ce => ce.Employee)
                        .ThenInclude(e => e.Department)
                    .Include(ce => ce.State);

                var courseEmployees = await courseEmployeesQuery
                    .Where(ce => ce.Course.CreatedEmployee.User.Id == userId).ToListAsync();

                var result = courseEmployees.Select(ce => new CourseEmployeeResponse
                {
                    Id = ce.Id,
                    CourseName = ce.Course.Name,
                    ExpirationDate = ce.Course.ExpirationDate,
                    CourseType = ce.Course.Type.DescriptionType,
                    Version = ce.Course.Version,
                    DetailDescription = ce.Course.DetailDescription,
                    EmployeeId = ce.Employee.User.Id,
                    EmployeeName = ce.Employee.User.Name,
                    CourseState = ce.State.DescriptionState,
                    CompletedDate = ce.CompletedDate,
                    CreatedBy = ce.Course.CreatedEmployee.User.Name + " " + ce.Course.CreatedEmployee.User.Surname,
                    Department = ce.Course.CreatedEmployee.Department.Name
                }).ToList();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("CloseCourseEmployee/{courseEmployeeId}")]
        [Authorize]
        public async Task<IActionResult> CloseCourse(string courseEmployeeId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var employee = await dbContext.EmployeeCards.Include(x => x.User).FirstAsync(x => x.User.Id == userId);

            if (!Guid.TryParse(userId, out Guid userGuid))
            {
                return BadRequest("Invalid user ID.");
            }

            var courseEmployee = await dbContext.CourseEmployees
                .Include(ce => ce.Employee)
                    .ThenInclude(e => e.User)
                .Include(ce => ce.State)
                .Include(ce => ce.Course)
                .FirstOrDefaultAsync(ce => ce.Course.Id.ToString() == courseEmployeeId && ce.Employee == employee);

            if (courseEmployee == null)
            {
                return NotFound(new { message = "CourseEmployee entry not found." });
            }

            var closedState = await dbContext.CourseStates.FirstOrDefaultAsync(cs => cs.DescriptionState == "Splnený");

            if (closedState == null)
            {
                return StatusCode(500, "State 'Splnený' not found in the database.");
            }

            courseEmployee.State = closedState;
            courseEmployee.CompletedDate = DateOnly.FromDateTime(DateTime.UtcNow);

            await dbContext.SaveChangesAsync();

            return Ok(new { message = "Course marked as closed." });
        }

        [HttpPost("StartCourseEmployee/{courseEmployeeId}")]
        [Authorize]
        public async Task<IActionResult> StartCourse(string courseEmployeeId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var employee = await dbContext.EmployeeCards.Include(x => x.User).FirstAsync(x => x.User.Id == userId);

            if (!Guid.TryParse(userId, out Guid userGuid))
            {
                return BadRequest("Invalid user ID.");
            }

            var courseEmployee = await dbContext.CourseEmployees
                .Include(ce => ce.Employee)
                    .ThenInclude(e => e.User)
                .Include(ce => ce.State)
                .Include(ce => ce.Course)
                .FirstOrDefaultAsync(ce => ce.Course.Id.ToString() == courseEmployeeId && ce.Employee == employee);

            if (courseEmployee == null)
            {
                return NotFound(new { message = "CourseEmployee entry not found." });
            }

            var newState = await dbContext.CourseStates.FirstOrDefaultAsync(cs => cs.DescriptionState == "Prebiehajúci");

            if (newState == null)
            {
                return StatusCode(500, "State 'Prebiehajúci' not found in the database.");
            }

            courseEmployee.State = newState;

            await dbContext.SaveChangesAsync();

            return Ok(new { message = "Course marked as closed." });
        }
    }
}
