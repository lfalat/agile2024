using AGILE2024_BE.Data;
using AGILE2024_BE.Models;
using AGILE2024_BE.Models.Identity;
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

        [HttpGet("UnarchivedOrganizations")]
        [Authorize(Roles = RolesDef.Spravca)]
        public async Task<IActionResult> UnarchivedOrganizations()
        {
            var organizations = await dbContext.Organizations
            .Include(o => o.RelatedDepartments)
            .Include(o => o.JobPositions)
            .Include(o => o.Location)
            .Where(o => !o.Archived)
            .ToListAsync();

            var organizationsResponse = new List<OrganizationResponse>();

            foreach (var organization in organizations)
            {
                organizationsResponse.Add(new OrganizationResponse
                {
                    Id = organization.Id,
                    Name = organization.Name,
                    Code = organization.Code,
                    LastEdited = organization.LastEdited,
                    Created = organization.Created,
                    Archived = organization.Archived,
                    LocationName = organization.Location?.Name

                });
            }

            return Ok(organizationsResponse);
        }
    }
}
