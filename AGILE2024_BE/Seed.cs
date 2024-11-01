using AGILE2024_BE.Data;
using AGILE2024_BE.Helpers;
using AGILE2024_BE.Models;
using System.Diagnostics.Metrics;

namespace AGILE2024_BE
{
    public class Seed
    {
        private readonly DataContext dataContext;
        public Seed(DataContext context)
        {
            this.dataContext = context;
        }
        public void SeedDataContext()// vlozenie testovacích dat do db
        {
            if (!dataContext.Users.Any())
            {
                var password = PasswordHasher.HashPassword("admin");
                var Users = new List<User>()
                {
                    new User()
                    {
                        Email = "admin@email.com",
                        Name = "Admin",
                        Surname = "Admin",
                        Title_after = " ",
                        Title_before = " ",
                        Password = password.HashedPassword,
                        Salt = password.Salt,
                        Role = new Role()
                        {
                            Label = "Spravca systemu",
                            Description = " "
                        }

                    },
                };
                dataContext.Users.AddRange(Users);
                
                var Roles = new List<Role>()
                {
                    new Role()
                    {
                        Label = "Zamestnanec",
                        Description = " "
                    },
                    new Role()
                    {
                        Label = "Power User",
                        Description = " "
                    },
                    new Role()
                    {
                        Label = "Veduci zamestnanec",
                        Description = " "
                    }
                };
                dataContext.Roles.AddRange(Roles);
                dataContext.SaveChanges();
            }
        }
    }
}

