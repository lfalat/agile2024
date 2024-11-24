using AGILE2024_BE.Data;
using AGILE2024_BE.Models;
using AGILE2024_BE.Models.Identity;
using AGILE2024_BE.Models.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;
using AGILE2024_BE.Models.Enums;
using Microsoft.EntityFrameworkCore;
using AGILE2024_BE.Models.Response.Feedback;
namespace AGILE2024_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private UserManager<ExtendedIdentityUser> userManager;
        private RoleManager<IdentityRole> roleManager;
        private IConfiguration config;
        private AgileDBContext dbContext;

        public FeedbackController(UserManager<ExtendedIdentityUser> um, IConfiguration co, RoleManager<IdentityRole> rm, AgileDBContext db)
        {
            this.userManager = um;
            this.config = co;
            this.roleManager = rm;
            this.dbContext = db;
        }

        [HttpPost]
        [Route("CreateFeedback")]
        public async Task<IActionResult> CreateFeedback([FromBody] FeedbackCreateResponse feedbackData)
        {
           if (feedbackData.questions == null || feedbackData.employees == null || feedbackData.questions.Count == 0 || feedbackData.employees.Count == 0)
            {
                return BadRequest();
            }
            var senderId = Guid.Parse(feedbackData.sender);
            var user = await userManager.FindByIdAsync(feedbackData.sender);
            if (user == null)
            {
                return NotFound("Not found user:" + feedbackData.sender);
            }
            FeedbackRequest feedbackRequest = new FeedbackRequest
            {
                sender = await dbContext.EmployeeCards.FirstOrDefaultAsync(e => e.User == user),
                title = "SV" + DateTime.Now.ToString("yyyyMMddHHmmss"),
                createDate = DateTime.Now
            };
            foreach (var question in feedbackData.questions)
            {
                FeedbackQuestion feedbackQuestion = new FeedbackQuestion
                {
                    text = question,
                    order = feedbackData.questions.IndexOf(question),
                    request = feedbackRequest
                };
                await dbContext.FeedbackQuestions.AddAsync(feedbackQuestion);
            }
            foreach (var employee in feedbackData.employees)
            {
                var employeeCard = await dbContext.EmployeeCards.FirstOrDefaultAsync(e => e.Id == Guid.Parse(employee));
                if (employeeCard == null)
                {
                    return NotFound("Not found employee:" + employee);
                }
                FeedbackRecipient feedbackRecipient = new FeedbackRecipient
                {
                    employee = employeeCard,
                    feedbackRequest = feedbackRequest,
                    status = await dbContext.FeedbackRequestStatuses.FirstOrDefaultAsync(e => e.description == EnumFeedbackRequestStatus.Nevyplnený.ToString()),
                    recievedDate = null,
                    isRead = false,
                };
                await dbContext.FeedbackRecipients.AddAsync(feedbackRecipient);
            }
            await dbContext.SaveChangesAsync();
            return Ok();
        }

    }
}
