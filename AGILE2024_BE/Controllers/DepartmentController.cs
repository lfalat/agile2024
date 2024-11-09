using AGILE2024_BE.Data;
using AGILE2024_BE.Models;
using AGILE2024_BE.Models.Identity;
using AGILE2024_BE.Models.Requests;
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
    [Authorize(Roles = RolesDef.Spravca)]
    public class DepartmentController : ControllerBase
    {
        private UserManager<ExtendedIdentityUser> userManager;
        private RoleManager<IdentityRole> roleManager;
        private IConfiguration config;
        private AgileDBContext dbContext;

        public DepartmentController(UserManager<ExtendedIdentityUser> um, IConfiguration co, RoleManager<IdentityRole> rm, AgileDBContext db)
        {
            this.userManager = um;
            this.config = co;
            this.roleManager = rm;
            this.dbContext = db;
        }

        [HttpGet("Departments")]
        [Authorize(Roles = RolesDef.Spravca)]
        public async Task<IActionResult> Departments()
        {
            var departments = await dbContext.Departments
            .Include(d => d.Organization)
            .ToListAsync();


            var departmentResponse = new List<DepartmentResponse>();

            foreach (var department in departments)
            {
                departmentResponse.Add(new DepartmentResponse
                {
                    Id = department.Id,
                    Name = department.Name,
                    Code = department.Code,
                    OrganizationName = department.Organization?.Name,
                    Created = department.Created,
                    LastEdited = department.LastEdited,
                    Archived = department.Archived,
                    ParentDepartmentId = department.ParentDepartment?.Id
                });
            }

            return Ok(departmentResponse);
        }

        [HttpGet("UnarchivedDepartments")]
        [Authorize(Roles = RolesDef.Spravca)]
        public async Task<IActionResult> UnarchivedDepartments()
        {
            var departments = await dbContext.Departments
            .Include(d => d.Organization)
             .Where(d => d.Archived != true)
            .ToListAsync();


            var departmentResponse = new List<DepartmentResponse>();

            foreach (var department in departments)
            {
                departmentResponse.Add(new DepartmentResponse
                {
                    Id = department.Id,
                    Name = department.Name,
                    Code = department.Code,
                    OrganizationName = department.Organization?.Name,
                    Created = department.Created,
                    LastEdited = department.LastEdited,
                    Archived = department.Archived,
                    ParentDepartmentId = department.ParentDepartment?.Id
                });
            }

            return Ok(departmentResponse);
        }

        [HttpPost("Create")]
        [Authorize(Roles = RolesDef.Spravca)]
        public async Task<IActionResult> Create([FromBody] CreateDepartmentRequest createRequest)
        {

            return Ok();
        }

        [HttpGet("{departmentId}")]
        [Authorize]
        public async Task<IActionResult> GetDepartmentById(Guid departmentId)
        {
            var department = await dbContext.Departments
        .Include(d => d.Organization)
        .Include(d => d.ParentDepartment)
        .FirstOrDefaultAsync(d => d.Id == departmentId);

            if (department == null)
            {
                return NotFound(new { message = "Oddelenie s týmto ID neexistuje." });
            }

            var departmentResponse = new DepartmentResponse
            {
                Id = department.Id,
                Name = department.Name,
                Code = department.Code,
                OrganizationName = department.Organization?.Name,
                Created = department.Created,
                LastEdited = department.LastEdited,
                Archived = department.Archived,
                ParentDepartmentId = department.ParentDepartment?.Id
            };

            return Ok(departmentResponse);
        }

        [HttpDelete("Delete/{departmentId}")]
        [Authorize(Roles = RolesDef.Spravca)]
        public async Task<IActionResult> Delete(Guid departmentId)
        {

            var department = await dbContext.Departments.FirstOrDefaultAsync(d => d.Id == departmentId);

            if (department == null)
            {
                return NotFound(new { message = "Department not found" });
            }

            bool hasRelatedEntities = await dbContext.EmployeeCards 
                               .AnyAsync(e => e.Department.Id == departmentId);

            if (hasRelatedEntities)
            {
                return BadRequest(new { message = "Cannot delete department as it is referenced in other records." });
            }

            dbContext.Departments.Remove(department);
            await dbContext.SaveChangesAsync();
            return Ok(new { message = "Department deleted successfully" });
        }

        [HttpPut("Unarchive/{departmentId}")]
        [Authorize(Roles = RolesDef.Spravca)]
        public async Task<IActionResult> Unarchive(Guid departmentId)
        {
            var department = await dbContext.Departments.FirstOrDefaultAsync(d => d.Id == departmentId);

            if (department == null)
            {
                return NotFound(new { message = "Department not found" });
            }

            department.Archived = false;
            await dbContext.SaveChangesAsync();

            return Ok(new { message = "Department archives successfully" });
        }

        [HttpPut("Archive/{departmentId}")]
        [Authorize(Roles = RolesDef.Spravca)]
        public async Task<IActionResult> Archive(Guid departmentId)
        {
            var department = await dbContext.Departments.FirstOrDefaultAsync(d => d.Id == departmentId);

            if (department == null)
            {
                return NotFound(new { message = "Department not found" });
            }

            department.Archived = true;
            await dbContext.SaveChangesAsync();


            return Ok(new { message = "Department unarchived successfully" });
        }
    }
}
