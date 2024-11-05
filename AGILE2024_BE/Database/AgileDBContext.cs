using AGILE2024_BE.Models;
using AGILE2024_BE.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AGILE2024_BE.Data
{
    /// <summary>
    /// Tato trieda služi ako databazovy context pre talent hub aplikaciu
    /// </summary>
    public class AgileDBContext : IdentityDbContext<ExtendedIdentityUser, IdentityRole, string>
    {
        public AgileDBContext(DbContextOptions options) : base(options)
        {

        }

        //Tento region služi na zadefinovane tabuliek v databaze
        //DbSet = tabuľka, typ v zobačikoch definuje stlpce v databaze
        #region TABLES
        public DbSet<ContractType> ContractTypes { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<EmployeeCard> EmployeeCards { get; set; }
        public DbSet<JobPosition> JobPositions { get; set; }
        public DbSet<Level> Levels { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        #endregion

    }
}
