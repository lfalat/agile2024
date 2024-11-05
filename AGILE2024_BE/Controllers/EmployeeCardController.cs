using AGILE2024_BE.Data;
using AGILE2024_BE.Models.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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
    }
}
