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
                    MaxErrors = 10,
                    MaxTime = 30,
                    IsDefault = true
                });
            }

            await context.SaveChangesAsync();
        }
    }
}