using AGILE2024_BE.Data;
using AGILE2024_BE.Models.Identity;
using AGILE2024_BE.Models.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AGILE2024_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeCardController : ControllerBase
    {
        private UserManager<ExtendedIdentityUser> userManager;
        private RoleManager<IdentityRole> roleManager;
        private IConfiguration config;
        private AgileDBContext dbContext;

        public EmployeeCardController(UserManager<ExtendedIdentityUser> um, IConfiguration co, RoleManager<IdentityRole> rm, AgileDBContext db)
        {
            this.userManager = um;
            this.config = co;
            this.roleManager = rm;
            this.dbContext = db;
        }

        [HttpGet("EmployeeCards")]
        [Authorize(Roles = RolesDef.Veduci)]
        public async Task<IActionResult> EmployeeCards()


        {
            ExtendedIdentityUser? user = await userManager.FindByEmailAsync(User.Identity?.Name!);
            var superiorId = user.Id; 

            if (string.IsNullOrEmpty(superiorId))
            {
                return Unauthorized("Nedokázali sme získať informácie o prihlásenom používateľovi.");
            }
            // Získanie zamestnancov, ktorí majú rolu zamestnanec a patria do oddelenia so zadaným superiorId
            var employeeCards = await dbContext.EmployeeCards
            .Include(ec => ec.User)  
            .Where(ec => ec.Department != null) 
            .Join(
                dbContext.UserRoles,  
                ec => ec.User.Id,      
                ur => ur.UserId,      
                (ec, ur) => new { EmployeeCard = ec, UserRole = ur }
            )
            .Join(
                dbContext.Roles,      
                combined => combined.UserRole.RoleId,
                r => r.Id,            
                (combined, role) => new { EmployeeCard = combined.EmployeeCard, Role = role, Department = combined.EmployeeCard.Department }
            )
            .Where(joined => joined.Role.Name == RolesDef.Zamestnanec)
            //.Where(joined => joined.Department.Superior.id == superiorId.ToString()) // Filtrovanie podľa SuperiorId v Department
            .Select(joined => new EmployeeCardResponse
        {
                    EmployeeId = joined.EmployeeCard.Id,
                    Email = joined.EmployeeCard.User.Email,
                    Name = joined.EmployeeCard.User.Name ?? string.Empty,
                    TitleBefore = joined.EmployeeCard.User.Title_before ?? string.Empty,
                    TitleAfter = joined.EmployeeCard.User.Title_after ?? string.Empty,
                    Department = joined.Department.Name ?? "N/A",
                    Surname = joined.EmployeeCard.User.Surname ?? string.Empty,
                    MiddleName = joined.EmployeeCard.User.MiddleName
            })
                .ToListAsync();

            return Ok(employeeCards);
        }
    }
}
