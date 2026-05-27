using LRouxTech.Core.Auth.Core.Interfaces;
using LRouxTech.Core.Auth.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LRouxTech.Core.Auth.Api.Extensions;

public static class UserModuleExtensions
{
    public static IServiceCollection AddUserModule(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();

        return services;
    }
}