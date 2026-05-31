using LRouxTech.Core.Auth.Core.Interfaces;
using LRouxTech.Core.Auth.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LRouxTech.Core.Auth.Api.Extensions;

public static class HttpContextModuleExtensions
{
    public static IServiceCollection AddHttpContextModule(this IServiceCollection services)
    {
        services.AddScoped<IHttpCurrentUserContext, HttpCurrentUserContext>();

        return services;
    }
}