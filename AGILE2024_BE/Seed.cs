using AGILE2024_BE.Data;
using AGILE2024_BE.Models;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics.Metrics;

namespace AGILE2024_BE
{
    public class Seed
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var _roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var _userManager = serviceProvider.GetRequiredService<UserManager<ExtendedIdentityUser>>();

            if (_roleManager != null && _userManager != null)
            {
                IdentityRole? role = await _roleManager.FindByNameAsync("Správca systému");
                if (role == null)
                {
                    await _roleManager.CreateAsync(new IdentityRole("Správca systému"));
                }

                IdentityRole? role2 = await _roleManager.FindByNameAsync("Zamestnanec");
                if (role2 == null)
                {
                    await _roleManager.CreateAsync(new IdentityRole("Zamestnanec"));
                }

                IdentityRole? role3 = await _roleManager.FindByNameAsync("Výkonný používateľ (Power User)");
                if (role3 == null)
                {
                    await _roleManager.CreateAsync(new IdentityRole("Výkonný používateľ (Power User)"));
                }

                IdentityRole? role4 = await _roleManager.FindByNameAsync("Vedúci zamestnanec");
                if (role4 == null)
                {
                    await _roleManager.CreateAsync(new IdentityRole("Vedúci zamestnanec"));
                }
          
                ExtendedIdentityUser? admin = await _userManager.FindByEmailAsync("admin@admin.com");
                if (admin == null)
                {
                    ExtendedIdentityUser newAdmin = new ExtendedIdentityUser()
                    {
                        UserName = "admin@admin.com",
                        Email = "admin@admin.com"
                    };

                    await _userManager.CreateAsync(newAdmin, "admin");

                    await _userManager.AddToRoleAsync(newAdmin, Roles.Spravca);
                }
            }
        }
    }
}

