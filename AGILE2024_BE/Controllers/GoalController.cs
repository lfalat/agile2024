using AGILE2024_BE.Data;
using AGILE2024_BE.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AGILE2024_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GoalController : ControllerBase
    {
        private UserManager<ExtendedIdentityUser> userManager;
        private RoleManager<IdentityRole> roleManager;
        private IConfiguration config;
        private AgileDBContext dbContext;

        public GoalController(UserManager<ExtendedIdentityUser> um, IConfiguration co,  RoleManager<IdentityRole> rm, AgileDBContext db)
        {
            this.userManager = um;
            this.config = co;
            this.roleManager = rm;
            this.dbContext = db;
        }

        [HttpGet("Goals")]
        [Authorize(Roles = RolesDef.Veduci)]
        public async Task<IActionResult> Organizations()
        {
            try
            {
                ExtendedIdentityUser? user = await userManager.FindByEmailAsync(User.Identity?.Name!);


                var goals1 = await dbContext.Goals
    .Include(g => g.employee)
    .ThenInclude(e => e.User)
    .Where(g => g.employee.User.Id == user.Id).ToListAsync();

                var goals = await dbContext.Goals
                     .Include(g => g.employee)
                    .ThenInclude(e => e.User)
                    .Include(g => g.category)
                    .Include(g => g.status)
                    .Where(g => g.employee.User.Id == user.Id) 
                    .Select(g => new
                    {
                        g.id,
                        g.name,
                        g.description,
                        categoryDescription = g.category.description,
                        statusDescription = g.status.description,
                        AssignedEmployees = dbContext.GoalAssignments
                        .Include(g => g.employee)
                        .ThenInclude(e => e.User)
                        .Include(g => g.goal)
                        .Where(ga => ga.goal.id == g.id)
                        .Select(ga => new
                    {
                        ga.employee.Id,
                        ga.employee.User.Name,
                        ga.employee.User.Surname
                    })
                    .ToList()
                    })
                    .ToListAsync();

                if (!goals.Any())
                {
                    return NoContent();
                }

                return Ok(goals);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Došlo k internej chybe pri načítavaní cieľov.");
            }
        }
    }
}
