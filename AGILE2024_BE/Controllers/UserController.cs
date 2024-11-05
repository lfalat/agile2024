using AGILE2024_BE.Helpers;
using AGILE2024_BE.Models.Identity;
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
    public class UserController : ControllerBase
    {
        private UserManager<ExtendedIdentityUser> userManager;
        private IConfiguration config;

        public UserController(UserManager<ExtendedIdentityUser> um, IConfiguration co)
        {
            this.userManager = um;
            this.config = co;
        }

        [HttpGet("Me")]
        [Authorize]
        public async Task<IActionResult> Me()
        {
            ExtendedIdentityUser? user = await userManager.FindByEmailAsync(User.Identity?.Name!);

            if (user == null)
            {
                return NotFound();
            }

            var roles = await userManager.GetRolesAsync(user);
            var userIdentityResponse = new UserIdentityResponse
            {
                id = user.Id,
                Email = user.Email!, // Email is never null
                FirstName = user.Name ?? string.Empty,
                LastName = user.Surname ?? string.Empty,
                TitleBefore = user.Title_before ?? string.Empty,
                TitleAfter = user.Title_after ?? string.Empty,
                Role = roles.FirstOrDefault()
            };

            return Ok(userIdentityResponse);
        }

        [HttpGet("Users")]
        [Authorize(Roles = RolesDef.Spravca)]
        public async Task<IActionResult> Users()
        {
            var users = await userManager.Users.ToListAsync();
            var userIdentityResponses = new List<UserIdentityResponse>();

            foreach (var user in users)
            {
                var roles = await userManager.GetRolesAsync(user);

                userIdentityResponses.Add(new UserIdentityResponse
                {
                    id = user.Id,
                    FirstName = user.Name,
                    LastName = user.Surname,
                    TitleBefore = user.Title_before,
                    TitleAfter = user.Title_after,
                    Email = user.Email!,
                    Role = roles.FirstOrDefault()
                });
            }

            return Ok(userIdentityResponses);
        }
    }
}
