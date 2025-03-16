using AGILE2024_BE.Data;
using AGILE2024_BE.Models;
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
        public async Task<IActionResult> MoveUp(string? userId)
        {
            //get department of user
            if (string.IsNullOrEmpty(userId))
            {
                return Ok("");
            }
            var orgUser = await dbContext.EmployeeCards
                .Include(ec => ec.User)
                .Include(ec => ec.Department)
                .Include(ec => ec.Department.Superior)
                .Include(ec => ec.Department.ParentDepartment)
                .Include(ec => ec.Department.ParentDepartment.Superior)
                .FirstOrDefaultAsync(ec => ec.User.Id == userId);
            if (orgUser.Department.ParentDepartment == null)
            {
                return Ok("");
            } else
            { 
                return Ok(orgUser.Department.ParentDepartment.Superior.Id);
            }
        }

        [HttpGet("Get0LevelOrganization")]
        public async Task<IActionResult> Get0LevelOrganization(string userId)
        {
            //Get organization of user
            var orgUser = await dbContext.EmployeeCards
                .Include(ec => ec.User)
                .Include(ec => ec.Department)
                .Include(ec => ec.Department.Organization)
                .Include(ec => ec.Department.Organization.Location)
                .FirstOrDefaultAsync(ec => ec.User.Id == userId);
            //1st level get departments of organization
            var departments = await dbContext.Departments
                .Include(d => d.Superior)
                .Include(d => d.Organization)
                .Where(d => d.Organization.Id == orgUser.Department.Organization.Id && d.ParentDepartment == null)
                .ToListAsync();
            // if organization has only one department return deparntment level
            if (departments.Count == 1)
            {
                return Ok(GetOneLevelHierarchy(departments[0].Id.ToString()).Result);
            }
            //get head users of departments
            TreeResponse tree = new TreeResponse
            {
                Id = orgUser.Department.Organization.Id,
                Name = orgUser.Department.Organization.Name,
                Code = orgUser.Department.Organization.Code,
                OrgTree = new List<OrgTreeNodeResponse>()
            };
            OrgTreeNodeResponse treeHead = new OrgTreeNodeResponse
            {
                EmplyeeCardId = null,
                UserId = null,
                Name = $"{orgUser.Department.Organization.Name} ({orgUser.Department.Organization.Code})",
                Location = $"{orgUser?.Department?.Organization?.Location?.Name} ({orgUser?.Department?.Organization?.Location?.Code})",
                Position = "",
            };
            tree.OrgTree.Add(treeHead);
            foreach (var department in departments)
            {
                var headUser = GetUser(department.Superior.Id, 1).Result;
                treeHead.children.Add(headUser);
            }
            return Ok(tree);
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
                headUser.children.Add(GetUser(user.User.Id, 1).Result);
            }

            foreach (var subDepartment in subDepartments)
            {
                headUser.children.Add(GetUser(subDepartment.Superior.Id, 1).Result);
            }

            return tree;
        }

        private async Task<OrgTreeNodeResponse> GetUser(string userId, int level = 0)
        {
            var orgUser = await dbContext.EmployeeCards
                .Include(ec => ec.User)
                .Include(ec => ec.Department)
                .Include(ec => ec.Level)
                .Include(ec => ec.Location)
                .Include(ec => ec.Level.JobPosition)
                .Include(ec => ec.Department.Superior)
                .FirstOrDefaultAsync(ec => ec.User.Id == userId);
            OrgTreeNodeResponse response = new OrgTreeNodeResponse
            {
                EmplyeeCardId = orgUser.Id,
                UserId = Guid.Parse(orgUser?.User?.Id),
                Name = orgUser.User.Name + " " + orgUser.User.Surname,
                Image = orgUser.User.ProfilePicLink,
                Location = $"{orgUser?.Location?.Name} ({orgUser?.Location?.Code})",
                isSuperior = orgUser?.Department?.Superior?.Id == orgUser.User.Id,
                Position = $"{orgUser?.Level.JobPosition?.Name} + ({orgUser?.Level?.Name})",
                level = level,
            };
            return response;
        }
    }
}
