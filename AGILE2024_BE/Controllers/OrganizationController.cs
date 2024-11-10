using AGILE2024_BE.Data;
using AGILE2024_BE.Models;
using AGILE2024_BE.Models.Identity;
using AGILE2024_BE.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

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
