using AGILE2024_BE.Helpers;
using AGILE2024_BE.Models;
using AGILE2024_BE.Models.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AGILE2024_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = RolesDef.Spravca)]
    public class RoleController : ControllerBase
    {
        private UserManager<ExtendedIdentityUser> userManager;
        private RoleManager<IdentityRole> roleManager;
        private IConfiguration config;

        public RoleController(UserManager<ExtendedIdentityUser> um, IConfiguration co, RoleManager<IdentityRole> roleManager)
        {
            this.userManager = um;
            this.config = co;
            this.roleManager = roleManager;
        }

        [HttpGet("Roles")]
        [Authorize(Roles = RolesDef.Spravca)]
        public async Task<IActionResult> Roles()
        {
            var roles = await roleManager.Roles.ToListAsync();

            return Ok(roles);
        }
    }
}
