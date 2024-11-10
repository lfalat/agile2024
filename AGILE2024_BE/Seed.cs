using AGILE2024_BE.Data;
using AGILE2024_BE.Models;
using AGILE2024_BE.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;

namespace AGILE2024_BE
{
    public class Seed
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var _roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var _userManager = serviceProvider.GetRequiredService<UserManager<ExtendedIdentityUser>>();
            var context = serviceProvider.GetRequiredService<AgileDBContext>();

            if (_roleManager != null && _userManager != null)
            {
                // Seed contract types
                var contractTypes = new List<ContractType>
                {
                    new ContractType { Name = "Dohoda o brigádnickej práci študentove" },
                    new ContractType { Name = "Trvalý pracovný pomer" },
                    new ContractType { Name = "Dohoda o vykonaní práce" },
                    new ContractType { Name = "Dohoda o pracovnej činnosti" },
                    new ContractType { Name = "Živnosť" },
                    new ContractType { Name = "Externý zamestnanec" }
                };

                foreach (var contractType in contractTypes)
                {
                    var existingContractType = await context.ContractTypes
                        .FirstOrDefaultAsync(ct => ct.Name == contractType.Name);

                    if (existingContractType == null)
                    {
                        await context.ContractTypes.AddAsync(contractType);
                    }
                }
                await context.SaveChangesAsync();

                // Seed roles
                IdentityRole? role = await _roleManager.FindByNameAsync(RolesDef.Spravca);
                if (role == null)
                {
                    await _roleManager.CreateAsync(new IdentityRole(RolesDef.Spravca));
                }

                IdentityRole? role2 = await _roleManager.FindByNameAsync(RolesDef.Zamestnanec);
                if (role2 == null)
                {
                    await _roleManager.CreateAsync(new IdentityRole(RolesDef.Zamestnanec));
                }

                IdentityRole? role3 = await _roleManager.FindByNameAsync(RolesDef.PowerUser);
                if (role3 == null)
                {
                    await _roleManager.CreateAsync(new IdentityRole(RolesDef.PowerUser));
                }

                IdentityRole? role4 = await _roleManager.FindByNameAsync(RolesDef.Veduci);
                if (role4 == null)
                {
                    await _roleManager.CreateAsync(new IdentityRole(RolesDef.Veduci));
                }

                ExtendedIdentityUser? admin = await _userManager.FindByEmailAsync("patrik@email.com");
                if (admin == null)
                {
                    string[] userNames = { "patrik", "brano", "lukas", "michal", "alexandra", "sara" };
                    string[] lastNames = { "balvan", "brano", "kišša", "ondrejka", "vojtasova", "papšova" };

                    for (int i = 0; i < userNames.Length; i++)
                    {
                        ExtendedIdentityUser user = new ExtendedIdentityUser()
                        {
                            UserName = $"{userNames[i]}@email.com",
                            Email = $"{userNames[i]}@email.com",
                            Name = userNames[i],
                            Surname = lastNames[i],
                            Title_before = "Bc."
                        };

                        await _userManager.CreateAsync(user, "AdminAdmin123!");
                        await _userManager.AddToRoleAsync(user, RolesDef.Spravca);
                    }
                }
                ExtendedIdentityUser? zam = await _userManager.FindByEmailAsync("zamestnanec@email.com");
                if (zam == null)
                {
                    ExtendedIdentityUser zamestnanec = new ExtendedIdentityUser()
                    {
                        UserName = "zamestnanec@email.com",
                        Email = "zamestnanec@email.com",
                        Name = "Meno",
                        Surname = "Priezvisko"
                    };

                    await _userManager.CreateAsync(zamestnanec, "AdminAdmin123!");
                    await _userManager.AddToRoleAsync(zamestnanec, RolesDef.Zamestnanec);
                }

                ExtendedIdentityUser? pu = await _userManager.FindByEmailAsync("poweruser@email.com");
                if (pu == null)
                {
                    ExtendedIdentityUser powerUser = new ExtendedIdentityUser()
                    {
                        UserName = "poweruser@email.com",
                        Email = "poweruser@email.com",
                        Name = "Meno",
                        Surname = "Priezvisko"
                    };

                    await _userManager.CreateAsync(powerUser, "AdminAdmin123!");
                    await _userManager.AddToRoleAsync(powerUser, RolesDef.PowerUser);
                }

                ExtendedIdentityUser? ved = await _userManager.FindByEmailAsync("veduci@email.com");
                if (ved == null)
                {
                    ExtendedIdentityUser veduci = new ExtendedIdentityUser()
                    {
                        UserName = "veduci@email.com",
                        Email = "veduci@email.com",
                        Name = "Meno",
                        Surname = "Priezvisko"
                    };

                    await _userManager.CreateAsync(veduci, "AdminAdmin123!");
                    await _userManager.AddToRoleAsync(veduci, RolesDef.Veduci);
                }
            }
        }
    }
}

