using AGILE2024_BE.Data;
using AGILE2024_BE.Models.Identity;
using AGILE2024_BE.Models.Requests.EmployeeCardRequests;
using AGILE2024_BE.Models.Response.TreeNode;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace AGILE2024_BE.Controllers.OrgHierarchyShow
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrgHierarhcyShowController : ControllerBase
    {
        private UserManager<ExtendedIdentityUser> userManager;
        private RoleManager<IdentityRole> roleManager;
        private IConfiguration config;
        private AgileDBContext dbContext;

        public OrgHierarhcyShowController(UserManager<ExtendedIdentityUser> um, IConfiguration co, RoleManager<IdentityRole> rm, AgileDBContext db)
        {
            this.userManager = um;
            this.config = co;
            this.roleManager = rm;
            this.dbContext = db;
        }
        [HttpGet("Get0LevelOrganization")]
        public async Task<IActionResult> Get0LevelOrganization(string orgId)
        {
            Guid guid = Guid.Parse(orgId);

            return Ok();
        }

        [HttpGet("GetLevelByID")]
        public async Task<IActionResult> GetLevelByID(string userId)
        {
            Guid guid = Guid.Parse(userId);
            var orgUser = await dbContext.EmployeeCards
                .Include(ec => ec.User)
                .Include(ec => ec.Department)
                .Include(ec => ec.Level)
                .Include(ec => ec.Location)
                .Include(ec => ec.Level.JobPosition)
                .FirstOrDefaultAsync(ec => ec.User.Id == userId);
            if (orgUser == null || orgUser.User == null)
            {
                return NotFound("Employee card not found.");
            }

            TreeResponse response = new TreeResponse
            {
                Id = orgUser.Id,
                Name = orgUser.User.Name,
                Code = orgUser.User.Email,
                OrgTree = new OrgTreeNodeResponse
                {
                    EmplyeeCardId = orgUser.Id,
                    Name = orgUser.User.Name,
                    Position = orgUser.Level.JobPosition.Name,
                    Subordinate = new List<OrgTreeNodeResponse>()
                }
            };
            return Ok(orgUser);



            return Ok();
        }

        public async Task<TreeResponse> GetOneLevelHierarchy(string superiorId)
        {
            var orgUser = await dbContext.EmployeeCards
                .Include(ec => ec.User)
                .Include(ec => ec.Department)
                .FirstOrDefaultAsync(ec => ec.User.Id == superiorId);

            TreeResponse response = new TreeResponse
            {
                Id = orgUser.Department.Id,
                Name = orgUser.Department.Name,
                Code = orgUser.Department.Code,
                OrgTree = GetUser(superiorId).Result
            };
            
            return new TreeResponse();
        }

        public async Task<OrgTreeNodeResponse> GetUser(string userId)
        {
            var orgUser = await dbContext.EmployeeCards
                .Include(ec => ec.User)
                .Include(ec => ec.Department)
                .Include(ec => ec.Level)
                .Include(ec => ec.Location)
                .Include(ec => ec.Level.JobPosition)
                .FirstOrDefaultAsync(ec => ec.User.Id == userId);
            OrgTreeNodeResponse response = new OrgTreeNodeResponse
            {
                EmplyeeCardId = orgUser.Id,
                UserId = Guid.Parse(orgUser.User.Id),
                Name = orgUser.User.Name,
                Location = orgUser.Location.Name,
                isSuperior = orgUser.Department.Superior.Id == orgUser.User.Id,
                Position = orgUser.Level.JobPosition.Name,
            };
            return response;
        }
    }
}
