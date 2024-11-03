using AGILE2024_BE.Models;
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
        //public DbSet<User> users 
        #endregion

    }
}
