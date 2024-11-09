using AGILE2024_BE.Data;
using AGILE2024_BE.Models;
using AGILE2024_BE.Models.Identity;
using AGILE2024_BE.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Web.Http.ModelBinding;

namespace AGILE2024_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrganizationController : ControllerBase
    {
        private UserManager<ExtendedIdentityUser> userManager;
        private RoleManager<IdentityRole> roleManager;
        private IConfiguration config;
        private AgileDBContext dbContext;

        public OrganizationController(UserManager<ExtendedIdentityUser> um, IConfiguration co, RoleManager<IdentityRole> rm, AgileDBContext db)
        {
            this.userManager = um;
            this.config = co;
            this.roleManager = rm;
            this.dbContext = db;
        }

        [HttpGet("Organizations")]
        [Authorize(Roles = RolesDef.Spravca)]
        public async Task<IActionResult> Organizations()
        {
            var organizations = await dbContext.Organizations.ToListAsync();

            return Ok(organizations);
        }

        [HttpPut("Archive")]
        [Authorize(Roles = RolesDef.Spravca)]
        public async Task<IActionResult> Archive([FromBody] ArchiveOrganizationRequest archiveRequest)
        {
            if (!ModelState.IsValid) return BadRequest();
            var organization = await dbContext.Organizations.FirstOrDefaultAsync(x => x.Id == archiveRequest.Id);

            if (organization == null)
            {
                return NotFound("Organization not found");
            }

            if (organization.Archived == archiveRequest.Archive)
            {
                return Ok();
            }

            organization.Archived = !organization.Archived;
            organization.LastEdited = DateTime.Now;
            dbContext.Organizations.Update(organization);
            await dbContext.SaveChangesAsync();
            if (organization.Archived)
            {
                return Ok("Organization has been archived");
            }
            else
            {
                return Ok("Organization has been unarchived");
            }
        }

        [HttpPost("Delete")]
        [Authorize(Roles = RolesDef.Spravca)]
        public async Task<IActionResult> Archive([FromBody] Guid selectedId)
        {
            if (selectedId == Guid.Empty)
            {
                return BadRequest("Invalid ID.");
            }

            var organization = await dbContext.Organizations.FirstOrDefaultAsync(x => x.Id == selectedId);

            if (organization == null)
            {
                return NotFound("Organization not found.");
            }

            var departments = await dbContext.Departments.Where(x => x.Organization.Id == selectedId).ToListAsync();
            foreach (var item in departments)
            {
                item.Organization = null;
            }
            dbContext.Departments.UpdateRange(departments);
            dbContext.Organizations.Remove(organization);
            await dbContext.SaveChangesAsync();

            return Ok("Organization deleted.");
        }


        [HttpPost("Register")]
        [Authorize(Roles = RolesDef.Spravca)]
        public async Task<IActionResult> Register([FromBody] RegisterOrganizationRequest registerRequest)
        {
            // Validate Name and Code
            if (string.IsNullOrWhiteSpace(registerRequest.Name) || string.IsNullOrWhiteSpace(registerRequest.Code))
            {
                return BadRequest("Both Name and Code are required.");
            }

            // Validate Location
            if (registerRequest.Location.IsNullOrEmpty())
            {
                return BadRequest("Location is required.");
            }

            var location = dbContext.Locations.FirstOrDefaultAsync(loc => loc.Id.ToString() == registerRequest.Location);
            if (location == null)
            {
                return BadRequest("Location invalid id");
            }

            Organization organization = new Organization
            {
                Name = registerRequest.Name,
                Code = registerRequest.Code,
                Location = location.Result,
                Created = DateTime.Now,
                LastEdited = DateTime.Now,
                Archived = false
            };

            try
            {
                // Add the organization to the database context
                await dbContext.Organizations.AddAsync(organization);

                // Save the changes to the database
                await dbContext.SaveChangesAsync();

                // Return a success response, including the ID of the new organization
                return Ok(new { Message = "Organization registered successfully.", OrganizationId = organization.Id });
            }
            catch (Exception ex)
            {
                //skurvilo sa to
                Console.Error.WriteLine($"Error while registering organization: {ex.Message}");
                return StatusCode(500, "An error occurred while registering the organization.");
            }
        }
    }
}
