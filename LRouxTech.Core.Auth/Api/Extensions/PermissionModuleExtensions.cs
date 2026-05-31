using LRouxTech.Core.Auth.Core.Interfaces;
using LRouxTech.Core.Auth.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LRouxTech.Core.Auth.Api.Extensions;

public static class PermissionModuleExtensions
{
    public static IServiceCollection AddPermissionModule(this IServiceCollection services)
    {
        services.AddScoped<IPermissionService, PermissionService>();

        return services;
    }
}