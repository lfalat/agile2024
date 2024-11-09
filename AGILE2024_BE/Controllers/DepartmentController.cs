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
             //.Where(d => d.Organization.RelatedDepartments.Any())
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
            try
            {
                if (createRequest == null)
                {
                    return BadRequest("Invalid data.");
                }
                

                var organization = await dbContext.Organizations
                .FirstOrDefaultAsync(o => o.Id.ToString() == createRequest.Organization);

                if (organization == null && createRequest.Organization != null)
                {
                    return BadRequest($"Organization with name {createRequest.Organization} does not exist.");
                }       


                var newDepartment = new Department
                {
                    Id = Guid.NewGuid(),
                    Name = createRequest.Name,
                    Code = createRequest.Code,
                    Organization = organization,
                    Archived = createRequest.Archived
                };
                dbContext.Departments.Add(newDepartment);

                //-------------------------------------

                var parentDepartment = await dbContext.Departments
                .FirstOrDefaultAsync(o => o.Id.ToString() == createRequest.ParentDepartmentId);

                //EDIT
                /*if (parentDepartment != null && parentDepartment.Id == newDepartment.Id)
                {
                    return BadRequest("Nadradené oddelenie nesmie byť rovnaké ako vkladané");
                }*/

                if (parentDepartment != null)
                {
                    newDepartment.ParentDepartment = parentDepartment;
                }
                await dbContext.SaveChangesAsync();

                //-------------------------------------

                if (createRequest.ChildDepartments != null && createRequest.ChildDepartments.Any())
                {
                    foreach (var childDepartmentId in createRequest.ChildDepartments)
                    {
                        var childDepartment = await dbContext.Departments
                            .FirstOrDefaultAsync(d => d.Id.ToString() == childDepartmentId);

                        if (childDepartment != null)
                        {
                                if (await IsAncestorAsync(childDepartment.Id, newDepartment.Id))
                                {
                                    return BadRequest("Cannot set a department as its own ancestor.");
                                }
                                childDepartment.ParentDepartment = newDepartment;
                                dbContext.Departments.Update(childDepartment);
                            
                        }
                    }
                    await dbContext.SaveChangesAsync();
                }

                //----------------------------------
                await dbContext.SaveChangesAsync();

                return Ok();
            }

            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error creating new employee record");
            }
        }

        private async Task<bool> IsAncestorAsync(Guid childId, Guid ancestorId)
        {
            var childDepartment = await dbContext.Departments
                .Include(d => d.ParentDepartment)
                .FirstOrDefaultAsync(d => d.Id == childId);
            var createdDepartment = await dbContext.Departments
                .FirstOrDefaultAsync(d => d.Id == ancestorId);
            var otec = createdDepartment.ParentDepartment;
            while (otec != null)
            {
                if (otec.Id == childDepartment.Id)
                {
                    return true;
                }

                otec = otec.ParentDepartment;
            }

            return false; 
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

            
            bool isReferencedInEmployeeCard = await dbContext.EmployeeCards 
                               .AnyAsync(e => e.Department.Id == departmentId);

            bool isReferencedInDepartment = await dbContext.Departments
                               .AnyAsync(e => e.ParentDepartment.Id == departmentId);

            if (isReferencedInEmployeeCard || isReferencedInDepartment)
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
