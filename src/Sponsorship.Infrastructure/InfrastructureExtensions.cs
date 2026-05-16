using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Sponsorship.Application.Interfaces;
using Sponsorship.Application.Interfaces.Repositories;
using Sponsorship.Infrastructure.Data;
using Sponsorship.Infrastructure.Repositories;
using Sponsorship.Infrastructure.Security;

namespace Sponsorship.Infrastructure;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        // Unit of Work — backed by the same scoped DbContext instance
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AppDbContext>());

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ISponsorshipRequestRepository, SponsorshipRequestRepository>();
        services.AddScoped<ISponsorshipTypeRepository, SponsorshipTypeRepository>();
        services.AddScoped<IWorkflowHistoryRepository, WorkflowHistoryRepository>();

        // Infrastructure services
        services.AddScoped<ITokenService, JwtTokenService>();
        services.AddScoped<IPasswordHasher, PasswordHasherService>();

        return services;
    }
}
