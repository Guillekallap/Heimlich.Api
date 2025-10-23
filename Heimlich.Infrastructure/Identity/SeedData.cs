using Heimlich.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Heimlich.Infrastructure.Identity
{
    public static class SeedData
    {
        public static async Task InitializeAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, HeimlichDbContext context)
        {
            string[] roles = new[] { "Instructor", "Practitioner", "Admin" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            if (!await context.Trunks.AnyAsync())
            {
                context.Trunks.AddRange(
                    new Trunk { Description = "Torso Adulto" },
                    new Trunk { Description = "Torso Adulto2" },
                    new Trunk { Description = "Torso Infante" }
                );
            }

            if (!await context.EvaluationConfigs.AnyAsync(c => c.IsDefault))
            {
                context.EvaluationConfigs.Add(new EvaluationConfig
                {
                    Name = "Default",
                    MaxErrors = 15,
                    MaxTime = 20,
                    MaxTimeInterval = 15,
                    IsDefault = true
                });
            }

            var sharedPassword = Environment.GetEnvironmentVariable("SEED_USER_PASSWORD") ?? "Univ123$";

            // Crear usuario admin si no existe
            if (await userManager.FindByNameAsync("admin") == null)
            {
                var user = new User
                {
                    UserName = "admin",
                    Email = "admin@gmaill.com",
                    Fullname = "admin"
                };
                await userManager.CreateAsync(user, sharedPassword); 
                await userManager.AddToRoleAsync(user, "Admin");
            }

            // Crear usuario instructor 1 si no existe
            if (await userManager.FindByNameAsync("JuanUnlam") == null)
            {
                var user = new User
                {
                    UserName = "JuanUnlam",
                    Email = "juanunlam@gmaill.com",
                    Fullname = "Juan Lopez"
                };
                await userManager.CreateAsync(user, sharedPassword); 
                await userManager.AddToRoleAsync(user, "Instructor");
            }

            // Crear usuario instructor 2 si no existe
            if (await userManager.FindByNameAsync("MariaUnlam") == null)
            {
                var user = new User
                {
                    UserName = "MariaUnlam",
                    Email = "mariaunlam@gmaill.com",
                    Fullname = "Maria Perez"
                };
                await userManager.CreateAsync(user, sharedPassword); 
                await userManager.AddToRoleAsync(user, "Instructor");
            }

            // Crear usuario practicante 1 si no existe
            if (await userManager.FindByNameAsync("GuillermoUnlam") == null)
            {
                var user = new User
                {
                    UserName = "GuillermoUnlam",
                    Email = "guillermounlam@gmaill.com",
                    Fullname = "Guillermo Callapina"
                };
                await userManager.CreateAsync(user, sharedPassword);
                await userManager.AddToRoleAsync(user, "Practitioner");
            }

            // Crear usuario practicante 2 si no existe
            if (await userManager.FindByNameAsync("CarlosUnlam") == null)
            {
                var user = new User
                {
                    UserName = "CarlosUnlam",
                    Email = "carlosunlam@gmaill.com",
                    Fullname = "Carlos Aguirre"
                };
                await userManager.CreateAsync(user, sharedPassword);
                await userManager.AddToRoleAsync(user, "Practitioner");
            }

            // Crear usuario practicante 3 si no existe
            if (await userManager.FindByNameAsync("LeonelUnlam") == null)
            {
                var user = new User
                {
                    UserName = "LeonelUnlam",
                    Email = "leonelunlam@gmaill.com",
                    Fullname = "Leonel Cespedes"
                };
                await userManager.CreateAsync(user, sharedPassword);
                await userManager.AddToRoleAsync(user, "Practitioner");
            }

            // Crear usuario practicante 4 si no existe
            if (await userManager.FindByNameAsync("DamianUnlam") == null)
            {
                var user = new User
                {
                    UserName = "DamianUnlam",
                    Email = "damianunlam@gmaill.com",
                    Fullname = "Damian Kuche"
                };
                await userManager.CreateAsync(user, sharedPassword);
                await userManager.AddToRoleAsync(user, "Practitioner");
            }

            // Crear usuario practicante 5 si no existe
            if (await userManager.FindByNameAsync("NicolasUnlam") == null)
            {
                var user = new User
                {
                    UserName = "NicolasUnlam",
                    Email = "nicolasunlam@gmaill.com",
                    Fullname = "Nicolas Diaz"
                };
                await userManager.CreateAsync(user, sharedPassword);
                await userManager.AddToRoleAsync(user, "Practitioner");
            }

            await context.SaveChangesAsync();
        }
    }
}