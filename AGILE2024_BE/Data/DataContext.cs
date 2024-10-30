using AGILE2024_BE.Models;
using Microsoft.EntityFrameworkCore;

namespace AGILE2024_BE.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        //potreba vytvoriť Dbset pre všetky tabuľky v databáze
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Potrebné zadefinovať primárne kľúče pri MxN vzťahoch aj jednotlivé vzťahy
            modelBuilder.Entity<User>()
                .HasKey(u => u.Id_user);

            modelBuilder.Entity<Role>()
                .HasKey(r => r.Id_role);
        }
    }
}
