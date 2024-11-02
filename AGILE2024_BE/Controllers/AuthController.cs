using AGILE2024_BE.Models;
using AGILE2024_BE.Models.Requests;
using AGILE2024_BE.Models.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AGILE2024_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private UserManager<ExtendedIdentityUser> userManager;
        private IConfiguration config;

        public AuthController(UserManager<ExtendedIdentityUser> um, IConfiguration co)
        {
            this.userManager = um;
            this.config = co;
        }

        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest refreshRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            ClaimsPrincipal? principal = GetTokenPrincipal(refreshRequest.JWTToken);
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
                JwtToken = await GenerateJWT(user),
                RefreshToken = user.RefreshToken
            };

            return Ok(refreshResponse);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            ExtendedIdentityUser? user = await userManager.FindByEmailAsync(loginRequest.Email);

            if (user == null)
            {
                return NotFound("Užívateľ s daným emailom neexistuje!");
            }

            bool passwordResult = await userManager.CheckPasswordAsync(user, loginRequest.Password);

            if (!passwordResult)
            {
                return Unauthorized("Heslo sa nezhoduje!");
            }

            LoginResponse loginResponse = new LoginResponse()
            {
                JwtToken = await GenerateJWT(user),
                RefreshToken = GenerateRT()
            };

            user.RefreshToken = loginResponse.RefreshToken;
            user.RefreshTokenExpiry = DateTime.Now.AddMinutes(30);
            await userManager.UpdateAsync(user);

            return Ok(loginResponse);
        }

        [HttpPost("Register")]
        [Authorize(Roles = Roles.Spravca)]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest registerRequest)
        {
            ExtendedIdentityUser newUser = new ExtendedIdentityUser()
            {
                UserName = registerRequest.Email,
                Email = registerRequest.Email,
                Name = registerRequest.Name,
                Surname = registerRequest.Surname
            };

            var result = await userManager.CreateAsync(newUser, registerRequest.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            await userManager.AddToRoleAsync(newUser, registerRequest.Role);

            return Ok();
        }

        private ClaimsPrincipal? GetTokenPrincipal(string token)
        {

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.GetSection("Jwt:Key").Value));

            var validation = new TokenValidationParameters
            {
                IssuerSigningKey = securityKey,
                ValidateLifetime = false,
                ValidateActor = false,
                ValidateIssuer = false,
                ValidateAudience = false,
            };
            return new JwtSecurityTokenHandler().ValidateToken(token, validation, out _);
        }

        private string GenerateRT()
        {
            var randomNumber = new byte[64];

            using (var numberGenerator = RandomNumberGenerator.Create())
            {
                numberGenerator.GetBytes(randomNumber);
            }

            return Convert.ToBase64String(randomNumber);
        }

        private async Task<string> GenerateJWT(ExtendedIdentityUser user)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Name, user.UserName!)
            };

            var userRoles = await userManager.GetRolesAsync(user);

            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var staticKey = config.GetSection("Jwt:Key").Value;
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(staticKey));
            var signingCred = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            var securityToken = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddMinutes(5),
                signingCredentials: signingCred,
                issuer: config.GetSection("Jwt:Issuer").Value,
                audience: config.GetSection("Jwt:Audience").Value
                );

            string tokenString = new JwtSecurityTokenHandler().WriteToken(securityToken);

            return tokenString;
        }
    }
}
