using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Sponsorship.Domain.Entities;
using Sponsorship.Domain.Enums;

namespace Sponsorship.Infrastructure.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(
        AppDbContext context,
        UserManager<User> userManager,
        RoleManager<IdentityRole<Guid>> roleManager)
    {
        await context.Database.MigrateAsync();

        if (await context.SponsorshipTypes.AnyAsync()) return;

        // ── Seed Identity roles ───────────────────────────────────────────────
        var allRoles = new[] { Roles.Requestor, Roles.Manager, Roles.FinanceAdmin, Roles.SystemAdmin };
        foreach (var roleName in allRoles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
                await roleManager.CreateAsync(new IdentityRole<Guid>(roleName) { Id = Guid.NewGuid() });
        }

        // ── Seed sponsorship types ─────────────────────────────────────────
        var types = new[]
        {
            new SponsorshipType { Name = "Conference" },
            new SponsorshipType { Name = "Community Event" },
            new SponsorshipType { Name = "Sports Event" },
            new SponsorshipType { Name = "Charity" },
            new SponsorshipType { Name = "Trade Show" }
        };
        context.SponsorshipTypes.AddRange(types);

        // ── Seed users and assign their Identity role ─────────────────────────
        var seedUsers = new[]
        {
            new { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Email = "requestor@test.com", FullName = "Alice Requestor", Department = "Marketing",  RoleName = Roles.Requestor    },
            new { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Email = "manager@test.com",   FullName = "Bob Manager",    Department = "Operations", RoleName = Roles.Manager      },
            new { Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), Email = "finance@test.com",   FullName = "Carol Finance",  Department = "Finance",    RoleName = Roles.FinanceAdmin },
            new { Id = Guid.Parse("44444444-4444-4444-4444-444444444444"), Email = "admin@test.com",     FullName = "Dave Admin",     Department = "IT",         RoleName = Roles.SystemAdmin  }
        };

        foreach (var s in seedUsers)
        {
            var user = await userManager.FindByEmailAsync(s.Email);
            if (user is null)
            {
                user = new User
                {
                    Id             = s.Id,
                    UserName       = s.Email,
                    Email          = s.Email,
                    EmailConfirmed = true,
                    FullName       = s.FullName,
                    Department     = s.Department,
                    CreatedAt      = DateTime.UtcNow
                };
                await userManager.CreateAsync(user, "Test@1234");
            }

            if (!await userManager.IsInRoleAsync(user, s.RoleName))
                await userManager.AddToRoleAsync(user, s.RoleName);
        }

        await context.SaveChangesAsync();
    }
}
