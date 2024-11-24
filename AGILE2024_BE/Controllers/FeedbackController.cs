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
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
namespace AGILE2024_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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
                    isReadBySender = false
                }; 
                await dbContext.FeedbackRecipients.AddAsync(feedbackRecipient);
            }
            await dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        [Route("ReadedRecipient")]
        public async Task<IActionResult> ReadedRecipient(string id)
        {
            var feedback = await dbContext.FeedbackRecipients.FirstOrDefaultAsync(e => e.id == Guid.Parse(id));
            if (feedback == null)
            {
                return NotFound("Not found feedback:" + id);
            }
            feedback.isRead = true;
            feedback.recievedDate = DateTime.Now;
            dbContext.FeedbackRecipients.Update(feedback);
            await dbContext.SaveChangesAsync();
            return Ok();
        }
        [HttpPost]
        [Route("ReadedSender")]
        public async Task<IActionResult> ReadedSender(string id)
        {
            var feedback = await dbContext.FeedbackRecipients.FirstOrDefaultAsync(e => e.id == Guid.Parse(id));
            if (feedback == null)
            {
                return NotFound("Not found feedback:" + id);
            }
            feedback.isReadBySender = true;
            dbContext.FeedbackRecipients.Update(feedback);
            await dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        [Route("DeclinedRecipient")]
        public async Task<IActionResult> DeclinedRecipient(string id)
        {
            var feedback = await dbContext.FeedbackRecipients.FirstOrDefaultAsync(e => e.id == Guid.Parse(id));
            if (feedback == null)
            {
                return NotFound("Not found feedback:" + id);
            }
            feedback.status = await dbContext.FeedbackRequestStatuses.FirstOrDefaultAsync(e => e.description == EnumFeedbackRequestStatus.Zamietnutý.ToString());
            feedback.sentDate = DateTime.Now;
            dbContext.FeedbackRecipients.Update(feedback);
            await dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpGet]
        [Route("GetDeliveredFeedbacks")]
        public async Task<IActionResult> GetDeliveredFeedbacks(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            var employee = await dbContext.EmployeeCards.FirstOrDefaultAsync(e => e.User == user);
            var feedbackRequests = await dbContext.FeedbackRequests
             .Where(fr => fr.sender == employee)
             .Select(fr => new
             {
                 FeedbackRequest = fr,
                 Recipients = dbContext.FeedbackRecipients
                     .Where(sfr => sfr.feedbackRequest == fr && sfr.sentDate != null)
                     .Include(sfr => sfr.employee)
                     .ThenInclude(sfr => sfr.User)
                     .Include(sfr => sfr.status)
                     .ToList()
             })
             .ToListAsync();
            List<FeedbackRecipientResponse> response = new List<FeedbackRecipientResponse>();
            foreach (var feedback in feedbackRequests)
            {
                foreach (var recipient in feedback.Recipients)
                {
                    response.Add(new FeedbackRecipientResponse
                    {
                        id = recipient.id.ToString(),
                        name = recipient.employee.User.Name + " " + recipient.employee.User.Surname,
                        email = recipient.employee.User.Email,
                        employeeId = recipient.employee.Id.ToString(),
                        recievedDate = recipient.recievedDate,
                        isRead = recipient.isReadBySender,
                        title = feedback.FeedbackRequest.title,
                        status = recipient.status.description,
                        createDate = feedback.FeedbackRequest.createDate,
                        sentDate = recipient.sentDate
                    });
                }
            }
            response = response.OrderBy(e => e.isRead).ToList();
            return Ok(response);
        }

        [HttpGet]
        [Route("GetRequiredFeedbacks")]
        public async Task<IActionResult> GetRequiredFeedbacks(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            var employee = await dbContext.EmployeeCards.FirstOrDefaultAsync(e => e.User == user);
            var feedbacks = await dbContext.FeedbackRecipients.Where(f => f.employee == employee)
                                                              .Include(f => f.employee)
                                                              .ThenInclude(f => f.User)
                                                              .Include(f => f.status)
                                                              .Include(f => f.feedbackRequest)
                                                              .ThenInclude(f => f.sender)
                                                              .ThenInclude(f => f.User)
                                                              .ToListAsync();
            List<FeedbackRecipientResponse> response = new List<FeedbackRecipientResponse>();
            foreach (var feedback in feedbacks)
            {
                response.Add(new FeedbackRecipientResponse
                {
                    id = feedback.id.ToString(),
                    name = feedback.feedbackRequest.sender.User.Name + " " + feedback.feedbackRequest.sender.User.Surname,
                    email = feedback.feedbackRequest.sender.User.Email,
                    employeeId = feedback.feedbackRequest.sender.Id.ToString(),
                    recievedDate = feedback.recievedDate,
                    isRead = feedback.isRead,
                    title = feedback.feedbackRequest.title,
                    status = feedback.status.description,
                    createDate = feedback.feedbackRequest.createDate,
                    sentDate = feedback.sentDate
                });
            }
            response = response.OrderBy(e => e.isRead).ToList();
            return Ok(response);
        }
        [HttpGet]
        [Route("GetFeedback")]
        public async Task<IActionResult> GetFeedback(string id)
        {
            var feedback = await dbContext.FeedbackRecipients
                                            .Include(fr => fr.employee)
                                            .ThenInclude(fr => fr.User)
                                            .Include(fr => fr.status)
                                            .Include(fr => fr.feedbackRequest)
                                            .FirstOrDefaultAsync(fr => fr.id == Guid.Parse(id)); 
            if (feedback == null)
            {
                return NotFound("Not found feedback:" + id);
            }
            var questions = await dbContext.FeedbackQuestions.Where(e => e.request == feedback.feedbackRequest).ToListAsync();
            List<FeedbackQuestionResponse> resQuestions = new List<FeedbackQuestionResponse>();
            foreach (var question in questions)
            {
                var answer = await dbContext.FeedbackAnswers.FirstOrDefaultAsync(e => e.request == question && e.recipient == feedback);
                resQuestions.Add(new FeedbackQuestionResponse
                {
                    id = question.id.ToString(),
                    text = question.text,
                    answer = answer == null ? "" : answer.text
                });
            }
            FeedbackRecipientResponse response = new FeedbackRecipientResponse
            {
                id = feedback.id.ToString(),
                name = feedback.employee.User.Name + " " + feedback.employee.User.Surname,
                email = feedback.employee.User.Email,
                employeeId = feedback.employee.Id.ToString(),
                recievedDate = feedback.recievedDate,
                isRead = feedback.isRead,
                title = feedback.feedbackRequest.title,
                status = feedback.status.description,
                createDate = feedback.feedbackRequest.createDate,
                sentDate = feedback.sentDate,
                feedbackQuestions = resQuestions
            };
            return Ok(response);
        }
        [HttpPost]
        [Route("AnswerFeedback")]
        public async Task<IActionResult> AnswerFeedback([FromBody] FeedbackRecipientResponse feedbackData)
        {
            var feedback = await dbContext.FeedbackRecipients.FirstOrDefaultAsync(e => e.id == Guid.Parse(feedbackData.id));
            if (feedback == null)
            {
                return NotFound("Not found feedback:" + feedbackData.id);
            }
            feedback.status = await dbContext.FeedbackRequestStatuses.FirstOrDefaultAsync(e => e.description == EnumFeedbackRequestStatus.Vyplnený.ToString());
            feedback.sentDate = DateTime.Now;
            dbContext.FeedbackRecipients.Update(feedback);
            foreach (var question in feedbackData.feedbackQuestions)
            {
                var feedbackQuestion = await dbContext.FeedbackQuestions.FirstOrDefaultAsync(e => e.id == Guid.Parse(question.id));
                if (feedbackQuestion == null)
                {
                    return NotFound("Not found question:" + question.id);
                }
                FeedbackAnswer feedbackAnswer = new FeedbackAnswer
                {
                    request = feedbackQuestion,
                    recipient = feedback,
                    text = question.answer
                };
                await dbContext.FeedbackAnswers.AddAsync(feedbackAnswer);
            }
            await dbContext.SaveChangesAsync();
            return Ok();
        }
    }
}
