using LRouxTech.Core.Auth.Core.Interfaces;
using LRouxTech.Core.Auth.Infrastructure.Validator;
using Microsoft.Extensions.DependencyInjection;

namespace LRouxTech.Core.Auth.Api.Extensions;

public static class ValidatorModuleExtensions
{
    public static IServiceCollection AddValidatorModule(this IServiceCollection services)
    {
        services.AddScoped<IUserValidator, UserValidator>();

        return services;
    }
}