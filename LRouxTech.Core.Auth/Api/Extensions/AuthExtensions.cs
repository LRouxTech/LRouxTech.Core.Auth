using LRouxTech.Core.Auth.Core.Interfaces;
using LRouxTech.Core.Auth.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LRouxTech.Core.Auth.Api.Extensions;

public static class AuthExtensions
{
    public static IServiceCollection AddAuthModule(this IServiceCollection services)
    {
        services.AddJwtAuth();
        services.AddUserModule();
        services.AddUserContext();
        return services;
    }
}