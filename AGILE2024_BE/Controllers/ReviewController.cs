using AGILE2024_BE.Models.Requests;
using AGILE2024_BE.Models.Response;
using AGILE2024_BE.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using AGILE2024_BE.Data;
using AGILE2024_BE.Models.Identity;
using AGILE2024_BE.Models.Requests.Review;
using Microsoft.EntityFrameworkCore;
using AGILE2024_BE.Models;
using Azure.Core;

namespace AGILE2024_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReviewContoller : ControllerBase
    {
        private UserManager<ExtendedIdentityUser> userManager;
        private RoleManager<IdentityRole> roleManager;
        private IConfiguration config;
        private AgileDBContext dbContext;

        public ReviewContoller(UserManager<ExtendedIdentityUser> um, IConfiguration co, RoleManager<IdentityRole> rm, AgileDBContext db)
        {
            this.userManager = um;
            this.config = co;
            this.roleManager = rm;
            this.dbContext = db;
        }

        [HttpPost]
        [Route("CreateReview")]
        public async Task<IActionResult> CreateReview([FromBody] ReviewCreateRequest request)
        {
            if (request == null || request.employees == null || !request.employees.Any())
            {
                return BadRequest("Invalid request data");
            }

            var review = new Review
            {
                id = Guid.NewGuid(),
                sender = await dbContext.EmployeeCards.FindAsync(request.SenderId),
                counter = await dbContext.Reviews.CountAsync() + 1,
                employeeEndDate = request.EmployeeDeadline,
                superiorEndDate = request.SuperiorDeadline,
                createDate = DateTime.UtcNow,
                endDate = null
            };

            dbContext.Reviews.Add(review);
            await dbContext.SaveChangesAsync();

            return Ok(new { message = "Review created successfully" });
        }

        [HttpGet]
        [Route("GetReviews")]
        public async Task<IActionResult> GetAllReviews()
        {
            var reviews = await dbContext.Reviews
                .Include(r => r.sender)
                .ToListAsync();

            var response = reviews.Select(r => new
            {
                Id = r.id,
                Name = $"Posudok č. PC{r.counter:D8}",
                Status = r.endDate.HasValue ? "Dokončený" : "Prebieha",
                AssignedEmployees = r.sender,
                CreatedAt = r.createDate,
                CompletedAt = r.endDate
            });

            return Ok(response);
        }
    }
}
