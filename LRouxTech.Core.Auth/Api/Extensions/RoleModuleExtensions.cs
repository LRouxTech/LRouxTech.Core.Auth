using LRouxTech.Core.Auth.Core.Interfaces;
using LRouxTech.Core.Auth.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LRouxTech.Core.Auth.Api.Extensions;

public static class RoleModuleExtensions
{
    public static IServiceCollection AddRoleModule(this IServiceCollection services)
    {
        services.AddScoped<IRoleService, RoleService>();

        return services;
    }
}