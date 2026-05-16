using Microsoft.Extensions.DependencyInjection;
using Sponsorship.Application.Interfaces;
using Sponsorship.Application.Services;

namespace Sponsorship.Application;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ISponsorshipRequestService, SponsorshipRequestService>();
        return services;
    }
}
