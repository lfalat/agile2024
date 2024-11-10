﻿using AGILE2024_BE.Data;
using AGILE2024_BE.Models;
using AGILE2024_BE.Models.Identity;
using AGILE2024_BE.Models.Requests;
using AGILE2024_BE.Models.Requests.User;
using AGILE2024_BE.Models.Response;
using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AGILE2024_BE.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private UserManager<ExtendedIdentityUser> userManager;
        private RoleManager<IdentityRole> roleManager;
        private IConfiguration config;
        private AgileDBContext dbContext;

        public ProfileController(UserManager<ExtendedIdentityUser> um, IConfiguration co, RoleManager<IdentityRole> rm, AgileDBContext db)
        {
            this.userManager = um;
            this.config = co;
            this.roleManager = rm;
            this.dbContext = db;
        }


        [HttpGet("ByUserId/{userId}")]
        [Authorize]
        public async Task<IActionResult> GetEmployeeCardByUserId(Guid userId)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return NotFound(new { message = "Používateľ s týmto ID neexistuje." });
            }
            var employeeCard = await dbContext.EmployeeCards
                .Include(e => e.User)
                .Include(e => e.Location)
                .Include(e => e.Department)
                .ThenInclude(d => d.Organization)
                .Include(e => e.Level)
                .Include(e => e.ContractType)

                .FirstOrDefaultAsync(e => e.User.Id == userId.ToString());


            if (employeeCard == null)
            {
                return NotFound(new { message = "Zamestnanecká karta pre tohto používateľa neexistuje." });
            }

            var employeeCardResponse = new EmployeeCardResponse
            {
                EmployeeId = employeeCard.Id,
                Email = employeeCard.User.Email,
                Birthdate = employeeCard.Birthdate?.ToString("yyyy-MM-dd"),
                Position = employeeCard.Level?.Name ?? "Neznáma pozícia",
                StartWorkDate = employeeCard.StartWorkDate?.ToString("yyyy-MM-dd"),
                Name = employeeCard.User.Name,
                TitleBefore = employeeCard.User.Title_before,
                TitleAfter = employeeCard.User.Title_after,
                Department = employeeCard.Department?.Name ?? "Nezaradené oddelenie",
                Organization = employeeCard.Department?.Organization?.Name ?? "Neznáma organizácia",
                ContractType = employeeCard.ContractType?.Name ?? "Neznámy typ zmluvy",
                WorkPercentage = employeeCard.WorkPercentage,
                Surname = employeeCard.User.Surname,
                Location = employeeCard.Location?.Name ?? "Neznáma lokalita",
                Archived = employeeCard.Archived,
                EmploymentDuration = employeeCard.StartWorkDate.HasValue
            ? GetEmploymentDuration(employeeCard.StartWorkDate.Value, DateTime.Now)
            : "Neznáma doba zamestnania",
                MiddleName = employeeCard.User.MiddleName
            };

            return Ok(employeeCardResponse);
        }



        [HttpPut("updateProfile/{userId}")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile(Guid userId, UpdateProfileRequest updateUserProfile)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return NotFound(new { message = "Používateľ s týmto ID neexistuje." });
            }

            user.Name = updateUserProfile.Name;
            user.Surname = updateUserProfile.Surname;
            user.MiddleName = updateUserProfile.MiddleName;

            var result = await userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return Ok(new { message = "Používateľské údaje boli úspešne aktualizované." });
            }
            else
            {
                return BadRequest(new { message = "Nastala chyba pri aktualizácii používateľských údajov." });
            }
        }

        [HttpPost("UploadPicture/{userId}")]
        public async Task<IActionResult> UploadPicture(Guid userId, [FromForm]UploadPictureRequest req)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());
            BlobServiceClient client = new(config.GetSection("Blob")["connectionString"]);
            var containerClient = client.GetBlobContainerClient("images");
            var blobClient = containerClient.GetBlobClient($"{Guid.NewGuid().ToString()}.{req.file.FileName.Split('.').Last()}");
            await blobClient.UploadAsync(req.file.OpenReadStream(), true);
            
            user.ProfilePicLink = blobClient.Uri.ToString();
            await userManager.UpdateAsync(user);

            return Ok();
        }


        string GetEmploymentDuration(DateTime startDate, DateTime endDate)
        {
            int years = endDate.Year - startDate.Year;
            int months = endDate.Month - startDate.Month;
            int days = endDate.Day - startDate.Day;

            if (days < 0)
            {
                months--;
                days += DateTime.DaysInMonth(startDate.Year, startDate.Month);
            }

            if (months < 0)
            {
                years--;
                months += 12;
            }

            return $"{years} rokov, {months} mesiacov, {days} dní";
        }
    }
    
}
