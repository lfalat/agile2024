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
    //[Authorize]
    public class OrgHierarchyShowController : ControllerBase
    {
        private UserManager<ExtendedIdentityUser> userManager;
        private RoleManager<IdentityRole> roleManager;
        private IConfiguration config;
        private AgileDBContext dbContext;

        public OrgHierarchyShowController(UserManager<ExtendedIdentityUser> um, IConfiguration co, RoleManager<IdentityRole> rm, AgileDBContext db)
        {
            this.userManager = um;
            this.config = co;
            this.roleManager = rm;
            this.dbContext = db;
        }

        [HttpGet("MoveUp")]
        public async Task<IActionResult> MoveUp(string userId)
        {
            //get department of user
            Guid guid = Guid.Parse(userId);
            var orgUser = await dbContext.EmployeeCards
                .Include(ec => ec.User)
                .Include(ec => ec.Department)
                .FirstOrDefaultAsync(ec => ec.User.Id == userId);
            if (orgUser == null || orgUser.User == null)
            {
                return NotFound("Employee card not found.");
            }
            //get superior of department
            if (orgUser.Department.Superior == null)
            {
                return Get0LevelOrganization(orgUser.Department.Organization.Id.ToString()).Result;
            }

            var supDepartment  = await dbContext.Departments
                .FirstOrDefaultAsync(d => d.Id.ToString() == orgUser.Department.Superior.Id);

            return Ok(GetOneLevelHierarchy(supDepartment.Id.ToString()).Result);
        }

        [HttpGet("Get0LevelOrganization")]
        public async Task<IActionResult> Get0LevelOrganization(string orgId)
        {
            Guid guid = Guid.Parse(orgId);
            //1st level get departments of organization
            //get head users of departments

            return Ok();
        }

        [HttpGet("GetLevelByID")]
        public async Task<IActionResult> GetLevelByID(string userId)
        {
            //get department of user
            var orgUser = await dbContext.EmployeeCards
                .Include(ec => ec.User)
                .Include(ec => ec.Department)
                .FirstOrDefaultAsync(ec => ec.User.Id == userId);
            if (orgUser == null || orgUser.User == null)
            {
                return NotFound("Employee card not found.");
            }

            TreeResponse response = GetOneLevelHierarchy(orgUser.Department.Id.ToString()).Result;
            return Ok(response);
        }

        private async Task<TreeResponse> GetOneLevelHierarchy(string departmentId)
        {
            //get department 
            var department = await dbContext.Departments
                .Include(d => d.Superior)
                .Include(d => d.Organization)
                .FirstOrDefaultAsync(d => d.Id.ToString() == departmentId);
            //get all users in department
            var orgUsers = await dbContext.EmployeeCards
                .Include(ec => ec.User)
                .Include(ec => ec.Department)
                .Where(ec => ec.Department.Id.ToString() == departmentId && ec.User.Id != department.Superior.Id)
                .ToListAsync();
            //get head users for sub-departments
            var subDepartments = await dbContext.Departments
                .Include(d => d.Superior)
                .Include(d => d.Organization)
                .Include(d => d.ParentDepartment)
                .Where(d => d.ParentDepartment.Id.ToString() == departmentId)
                .ToListAsync();
            var headUser = GetUser(department.Superior.Id).Result;
            TreeResponse tree = new TreeResponse
            {
                Id = department.Id,
                Name = department.Name,
                Code = department.Code,
                OrgTree = new List<OrgTreeNodeResponse> { headUser }
            };

            foreach (var user in orgUsers)
            {
                headUser.children.Add(GetUser(user.User.Id).Result);
            }

            foreach (var subDepartment in subDepartments)
            {
                headUser.children.Add(GetUser(subDepartment.Superior.Id).Result);
            }

            return tree;
        }

        private async Task<OrgTreeNodeResponse> GetUser(string userId)
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
                Name = orgUser.User.Name + " " + orgUser.User.Surname,
                Location = $"{orgUser.Location.Name} ({orgUser.Location.Code})",
                isSuperior = orgUser.Department.Superior.Id == orgUser.User.Id,
                Position = $"{orgUser.Level.JobPosition.Name} + ({orgUser.Level.Name})",
            };
            return response;
        }
    }
}
