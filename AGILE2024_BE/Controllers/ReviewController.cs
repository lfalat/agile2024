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
    public class ReviewController : ControllerBase
    {
        private UserManager<ExtendedIdentityUser> userManager;
        private RoleManager<IdentityRole> roleManager;
        private IConfiguration config;
        private AgileDBContext dbContext;

        public ReviewController(UserManager<ExtendedIdentityUser> um, IConfiguration co, RoleManager<IdentityRole> rm, AgileDBContext db)
        {
            this.userManager = um;
            this.config = co;
            this.roleManager = rm;
            this.dbContext = db;
        }

        [HttpGet("Count")]
        public async Task<IActionResult> GetReviewCount()
        {
            try
            {
                int reviewCount = await dbContext.Reviews.CountAsync();
                return Ok(reviewCount);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Chyba servera pri získavaní počtu posudkov.");
            }
        }

        [HttpPost("CreateReview")]
        public async Task<IActionResult> CreateReview([FromBody] ReviewCreateRequest request)
        {
            if (request == null || request.employeeIds == null || !request.employeeIds.Any())
            {
                return BadRequest("Invalid request data");
            }

            var senderUser = await dbContext.Users
                .FirstOrDefaultAsync(u => u.Id == request.SenderId.ToString());

            if (senderUser == null)
            {
                return BadRequest("Sender user not found");
            }

            var senderEmployeeCard = await dbContext.EmployeeCards
                .FirstOrDefaultAsync(ec => ec.User.Id == senderUser.Id);

            if (senderEmployeeCard == null)
            {
                return BadRequest("Employee card for sender not found");
            }

            Guid idReview = Guid.NewGuid();
            var review = new Review
            {
                id = idReview,
                sender = senderEmployeeCard,
                counter = await dbContext.Reviews.CountAsync() + 1,
                employeeEndDate = request.EmployeeDeadline,
                superiorEndDate = request.SuperiorDeadline,
                createDate = DateTime.UtcNow,
                endDate = null
            };

            dbContext.Reviews.Add(review);
            await dbContext.SaveChangesAsync();

            var goalAssignments = await dbContext.GoalAssignments
                .Where(ga => request.employeeIds.Contains(ga.employee.Id.ToString()))                  
                .ToListAsync();

            if (!goalAssignments.Any())
            {
                return BadRequest("No GoalAssignments found for the provided employee IDs.");
            }

            var reviewRecipients = goalAssignments.Select(ga => new ReviewRecipient
            {
                id = Guid.NewGuid(), 
                review = review,  
                goalAssignment = ga,  
                superiorDescription = null, 
                employeeDescription = null  
            }).ToList();

            dbContext.ReviewRecipents.AddRange(reviewRecipients);
            await dbContext.SaveChangesAsync();

            return Ok(new { message = "Review created successfully" });
        }


        [HttpGet("GetReviews")]
        public async Task<IActionResult> GetAllReviews()
        {
            var reviewRecipents = await dbContext.ReviewRecipents
                .Include(r => r.review) 
                .Include(r => r.goalAssignment) 
                .ThenInclude(ga => ga.employee)
                .ThenInclude(emp => emp.User)  
                .ToListAsync();

            var groupedReviews = reviewRecipents
                .GroupBy(rr => rr.review.id)   
               .Select(group => new
               {
                   id = group.Key,  
                   ReviewId = group.Key,  
                   ReviewName = $"Posudok č. PC{group.FirstOrDefault()?.review.counter:D8}",  
                   Status = group.FirstOrDefault()?.review.endDate.HasValue == true ? "Dokončený" : "Prebieha", 
                   AssignedEmployees = group
                        .Select(rr => new
                        {
                            id = rr.goalAssignment?.employee.Id,  
                            name = rr.goalAssignment?.employee.User.Name + " " + rr.goalAssignment?.employee.User.Surname                 })
                        .DistinctBy(emp => emp.id)  
                        .ToList(), 
                   CreatedAt = group.FirstOrDefault()?.review.createDate,  
                   CompletedAt = group.FirstOrDefault()?.review.endDate  
               })
               .ToList();

            return Ok(groupedReviews);
        }

    }
}
