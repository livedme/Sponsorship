using Microsoft.EntityFrameworkCore;
using Sponsorship.Domain.Entities;
using Sponsorship.Domain.Enums;
using Sponsorship.Infrastructure.Security;

namespace Sponsorship.Infrastructure.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        await context.Database.MigrateAsync();

        if (await context.SponsorshipTypes.AnyAsync()) return;

        var types = new[]
        {
            new SponsorshipType { Name = "Conference" },
            new SponsorshipType { Name = "Community Event" },
            new SponsorshipType { Name = "Sports Event" },
            new SponsorshipType { Name = "Charity" },
            new SponsorshipType { Name = "Trade Show" }
        };
        context.SponsorshipTypes.AddRange(types);

        var users = new[]
        {
            new User
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Email = "requestor@test.com",
                PasswordHash = PasswordHasher.Hash("Test@1234"),
                FullName = "Alice Requestor",
                Department = "Marketing",
                Role = UserRole.Requestor
            },
            new User
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Email = "manager@test.com",
                PasswordHash = PasswordHasher.Hash("Test@1234"),
                FullName = "Bob Manager",
                Department = "Operations",
                Role = UserRole.Manager
            },
            new User
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                Email = "finance@test.com",
                PasswordHash = PasswordHasher.Hash("Test@1234"),
                FullName = "Carol Finance",
                Department = "Finance",
                Role = UserRole.FinanceAdmin
            },
            new User
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                Email = "admin@test.com",
                PasswordHash = PasswordHasher.Hash("Test@1234"),
                FullName = "Dave Admin",
                Department = "IT",
                Role = UserRole.SystemAdmin
            }
        };
        context.Users.AddRange(users);
        await context.SaveChangesAsync();
    }
}
