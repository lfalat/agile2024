using AGILE2024_BE.Data;
using AGILE2024_BE.Models.Identity;
using AGILE2024_BE.Models.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AGILE2024_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LevelController : ControllerBase
    {
        private UserManager<ExtendedIdentityUser> userManager;
        private RoleManager<IdentityRole> roleManager;
        private IConfiguration config;
        private AgileDBContext dbContext;

        public LevelController(UserManager<ExtendedIdentityUser> um, IConfiguration co, RoleManager<IdentityRole> rm, AgileDBContext db)
        {
            this.userManager = um;
            this.config = co;
            this.roleManager = rm;
            this.dbContext = db;
        }
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var levels = await dbContext.Levels.Include(x=> x.JobPosition).ToListAsync();

            var levelResponses = levels.Select(level => new LevelResponse
            {
                Id = level.Id.ToString(),
                Name = level.Name,
                JobPosition = level.JobPosition?.Id.ToString() ?? ""
            }).ToList(); 

            return Ok(levelResponses);
        }


        [HttpGet("Get")]
        public async Task<IActionResult> Get(Guid jobPostionID)
        {
            var levels = dbContext.Levels.Where( x => x.JobPosition.Id == jobPostionID ).ToList();
            return Ok(levels);
        }
    }
}
