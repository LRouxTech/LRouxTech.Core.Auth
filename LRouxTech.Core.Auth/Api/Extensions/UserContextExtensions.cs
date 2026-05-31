using LRouxTech.Core.Auth.Core.Interfaces;
using LRouxTech.Core.Auth.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LRouxTech.Core.Auth.Api.Extensions;

public static class UserContextExtensions
{
    public static IServiceCollection AddUserContext(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddHttpContextAccessor();
        services.AddScoped<IHttpCurrentUserContext, HttpCurrentUserContext>();
        return services;
    }
}