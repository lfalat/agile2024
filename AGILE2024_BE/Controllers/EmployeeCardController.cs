﻿using AGILE2024_BE.Data;
using AGILE2024_BE.Models;
using AGILE2024_BE.Models.Identity;
using AGILE2024_BE.Models.Requests;
using AGILE2024_BE.Models.Requests.EmployeeCardRequests;
using AGILE2024_BE.Models.Requests.JobPositionRequests;
using AGILE2024_BE.Models.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace AGILE2024_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = RolesDef.Spravca)]
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

        [HttpPost("Update")]
        [Authorize(Roles = RolesDef.Spravca)]
        public async Task<IActionResult> Update([FromBody] UpdateEmployeeCardRequest employeeCardRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await dbContext.Users.FindAsync(employeeCardRequest.UserId);
            var employeeCard = await dbContext.EmployeeCards
                .FindAsync(employeeCardRequest.Id);

            var level = await dbContext.Levels.FindAsync(employeeCardRequest.Level);
            var location = await dbContext.Locations.FindAsync(employeeCardRequest.Location);
            var department = await dbContext.Departments.FindAsync(employeeCardRequest.Department);
            var contractType = await dbContext.ContractTypes.FindAsync(employeeCardRequest.ContractType);
            
            employeeCard.Birthdate = DateTime.Parse(employeeCardRequest.Birth);
            employeeCard.ContractType = contractType;
            employeeCard.Location = location;
            employeeCard.Department = department;
            employeeCard.WorkPercentage = employeeCardRequest.WorkTime;
            employeeCard.Level = level;
            employeeCard.StartWorkDate = DateTime.Parse(employeeCardRequest.StartWorkDate);
            employeeCard.LastEdited = DateTime.Now;

            dbContext.EmployeeCards.Update(employeeCard);

            await dbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("EmployeeCards")]
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

        [HttpPost("Deactivate/{userId}")]
        public async Task<IActionResult> Deactivate(string userId)
        {
            var employeeCard = await dbContext.EmployeeCards.FirstAsync(x => x.User.Id == userId);

            employeeCard.Archived =! employeeCard.Archived;

            dbContext.EmployeeCards.Update(employeeCard);

            await dbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var employeeCard = await dbContext.EmployeeCards.Include(x => x.User).ToListAsync();

            return Ok(employeeCard);
    }
}
