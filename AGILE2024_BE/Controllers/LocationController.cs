﻿using AGILE2024_BE.Data;
using AGILE2024_BE.Models;
using AGILE2024_BE.Models.Identity;
using AGILE2024_BE.Models.Requests;
using AGILE2024_BE.Models.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Reflection.Emit;
using System.Xml.Linq;

namespace AGILE2024_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class LocationController : ControllerBase
    {
        private UserManager<ExtendedIdentityUser> userManager;
        private RoleManager<IdentityRole> roleManager;
        private IConfiguration config;
        private AgileDBContext dbContext;

        public LocationController(UserManager<ExtendedIdentityUser> um, IConfiguration co, RoleManager<IdentityRole> rm, AgileDBContext db)
        {
            this.userManager = um;
            this.config = co;
            this.roleManager = rm;
            this.dbContext = db;
        }

        [HttpGet("Locations")]
        [Authorize(Roles = RolesDef.Spravca + ", " + RolesDef.Veduci + ", " + RolesDef.Zamestnanec)]
        public async Task<IActionResult> Locations()
        {
            var locations = await dbContext.Locations
                
                .Select(location => new LocationResponse
        {
            Id = location.Id,
            Name = location.Name,
            Code = location.Code,
            City = location.City,
            ZipCode = location.ZipCode,
            Adress = location.Adress,
            Latitude = location.Latitude,
            Longitude = location.Longitude,
            Created = location.Created,
            LastEdited = location.LastEdited,
            Archived = location.Archived,
            Organizations = dbContext.Organizations
                .Where(org => org.Location != null  && org.Location.Id == location.Id)
                .Select(org => org.Name)
                .ToList()
        })
        .ToListAsync();


            return Ok(locations);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = RolesDef.Spravca)]
        public async Task<IActionResult> GetLocation(Guid id)
        {
            var location = await dbContext.Locations.FindAsync(id);
            if (location == null)
            {
                return NotFound(new { message = "Location not found" });
            }
            return Ok(location);
        }

        [HttpPost("Create")]
        [Authorize(Roles = RolesDef.Spravca)]
        public async Task<IActionResult> Create([FromBody] CreateLocationRequest createRequest)
        {
            if (createRequest == null)
                {
                    return BadRequest("Invalid data.");
                }

            var newLocation = new Location
            {
                Name = createRequest.Name,
                Code = createRequest.Code,
                City = createRequest.City,
                ZipCode = createRequest.ZipCode,
                Adress = createRequest.Adress,

            };

            if (createRequest.Latitude != 0 ) {
                newLocation.Latitude = createRequest.Latitude;
            }

            if (createRequest.Longitude != 0)
            {
                newLocation.Longitude = createRequest.Longitude;
            }
            

            try
            {             
                await dbContext.Locations.AddAsync(newLocation);
                await dbContext.SaveChangesAsync();

                if (createRequest.Organizations.Any())
                {
                    var organizations = dbContext.Organizations
                        .Where(o => createRequest.Organizations.Contains(o.Name))
                        .ToList();

                    foreach (var org in organizations)
                    {
                        org.Location = newLocation;
                    }

                    await dbContext.SaveChangesAsync();
                }

                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Problem vytvorenia novej lokacie");
            }
        }

        [HttpPut("Update/{id}")]
        [Authorize(Roles = RolesDef.Spravca)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateLocationRequest locationRequest)
        {
            if (locationRequest == null)
            {
                return BadRequest("Invalid data.");
            }

            var existingLocation = await dbContext.Locations.FindAsync(id);
            if (existingLocation == null)
            {
                return NotFound("Location not found.");
            }

            existingLocation.Name = locationRequest.Name;
            existingLocation.Code = locationRequest.Code;
            existingLocation.City = locationRequest.City;
            existingLocation.ZipCode = locationRequest.ZipCode;
            existingLocation.Adress = locationRequest.Adress;
            existingLocation.Longitude = locationRequest.Longitude;
            existingLocation.Latitude = locationRequest.Latitude;
            existingLocation.LastEdited = DateTime.UtcNow;

            var existingOrganizations = dbContext.Organizations.Where(o => o.Location.Id == id).ToList();
            
            var organizationsToRemove = existingOrganizations
                .Where(o => !locationRequest.Organizations.Contains(o.Name))
                .ToList();

            foreach (var org in organizationsToRemove)
            {
                org.Location = null; 
            }

            foreach (var orgName in locationRequest.Organizations)
            {
                var organization = existingOrganizations.FirstOrDefault(o => o.Name == orgName);
                if (organization == null)
                {
                    // Add new organization if it doesn't exist
                    organization = await dbContext.Organizations.FirstOrDefaultAsync(o => o.Name == orgName);
                    if (organization != null)
                    {
                        organization.Location = existingLocation;
                    }
                }
            }
            try
            {
                await dbContext.SaveChangesAsync();
                return Ok("Location updated successfully.");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Problem updating location.");
            }
        }


        [HttpDelete("Delete/{locationId}")]
        [Authorize(Roles = RolesDef.Spravca)]
        public async Task<IActionResult> Delete(Guid locationId)
        {

            var location = await dbContext.Locations.FirstOrDefaultAsync(d => d.Id == locationId);

            if (location == null)
            {
                return NotFound(new { message = "Department not found" });
            }


            bool isReferencedInOrganization = await dbContext.Organizations
                               .AnyAsync(e => e.Location.Id == locationId);

            if (isReferencedInOrganization)
            {
                return BadRequest(new { message = "Cannot delete location as it is referenced in other records." });
            }

            dbContext.Locations.Remove(location);
            await dbContext.SaveChangesAsync();
            return Ok(new { message = "Location deleted successfully" });
        }


        [HttpPut("Unarchive/{locationId}")]
        [Authorize(Roles = RolesDef.Spravca)]
        public async Task<IActionResult> Unarchive(Guid locationId)
        {
            var location = await dbContext.Locations.FirstOrDefaultAsync(d => d.Id == locationId);

            if (location == null)
            {
                return NotFound(new { message = "Location not found" });
            }

            location.Archived = false;
            await dbContext.SaveChangesAsync();

            return Ok(new { message = "Location archives successfully" });
        }

        [HttpPut("Archive/{locationId}")]
        [Authorize(Roles = RolesDef.Spravca)]
        public async Task<IActionResult> Archive(Guid locationId)
        {
            var location = await dbContext.Locations.FirstOrDefaultAsync(d => d.Id == locationId);

            if (location == null)
            {
                return NotFound(new { message = "Location not found" });
            }

            location.Archived = true;
            await dbContext.SaveChangesAsync();


            return Ok(new { message = "Location unarchived successfully" });
        }

        [HttpGet("GetAllUnarchived")]
        public async Task<IActionResult> GetAllUnarchived()
        {
            var result = await dbContext.Locations.Where(x => x.Archived == false).ToListAsync();

            return Ok(result);
        }
    }
}
