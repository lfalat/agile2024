using AGILE2024_BE.Models.Requests;
using AGILE2024_BE.Models.Response;
using AGILE2024_BE.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using AGILE2024_BE.Data;
using AGILE2024_BE.Models.Identity;

namespace AGILE2024_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private UserManager<ExtendedIdentityUser> userManager;
        private RoleManager<IdentityRole> roleManager;
        private IConfiguration config;
        private AgileDBContext dbContext;

        public AuthController(UserManager<ExtendedIdentityUser> um, IConfiguration co, RoleManager<IdentityRole> rm, AgileDBContext db)
        {
            this.userManager = um;
            this.config = co;
            this.roleManager = rm;
            this.dbContext = db;
        }

        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest refreshRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            ClaimsPrincipal? principal = Auth.GetTokenPrincipal(refreshRequest.JWTToken, config);
            if (principal?.Identity?.Name == null)
            {
                return NotFound();
            }

            ExtendedIdentityUser? user = await userManager.FindByNameAsync(principal.Identity.Name);

            if (user == null || user.RefreshToken != refreshRequest.RefreshToken || user.RefreshTokenExpiry < DateTime.Now)
            {
                return Unauthorized();
            }

            LoginResponse refreshResponse = new LoginResponse()
            {
                NewJwtToken = await Auth.GenerateJWT(user, userManager, config),
                NewRefreshToken = user.RefreshToken
            };

            return Ok(refreshResponse);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginUserRequest loginRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            ExtendedIdentityUser? user = await userManager.FindByEmailAsync(loginRequest.Email);

            if (user == null || !(await userManager.CheckPasswordAsync(user, loginRequest.Password)))
            {
                return NotFound("Zadali ste nesprávne údaje!");
            }

            LoginResponse loginResponse = new LoginResponse()
            {
                NewJwtToken = await Auth.GenerateJWT(user, userManager, config),
                NewRefreshToken = Auth.GenerateRT()
            };

            user.RefreshToken = loginResponse.NewRefreshToken;
            user.RefreshTokenExpiry = DateTime.Now.AddMinutes(30);
            await userManager.UpdateAsync(user);

            return Ok(loginResponse);
        }

        [HttpPost("Register")]
        [Authorize(Roles = RolesDef.Spravca)]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest registerRequest)
        {
            ExtendedIdentityUser newUser = new ExtendedIdentityUser()
            {
                UserName = registerRequest.Email,
                Email = registerRequest.Email,
                Name = registerRequest.Name,
                Surname = registerRequest.Surname,
                Title_before = registerRequest.Title_Before,
                Title_after = registerRequest.Title_After
            };

            var result = await userManager.CreateAsync(newUser, registerRequest.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            await userManager.AddToRoleAsync(newUser, registerRequest.Role);

            return Ok();
        }
    }
}
