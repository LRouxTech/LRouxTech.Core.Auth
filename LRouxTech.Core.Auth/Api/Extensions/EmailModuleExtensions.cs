using LRouxTech.Core.Auth.Core.Interfaces;
using LRouxTech.Core.Auth.Infrastructure.Validator;
using LRouxTech.Core.Mail;
using Microsoft.Extensions.DependencyInjection;

namespace LRouxTech.Core.Auth.Api.Extensions;

public static class EmailModuleExtensions
{
    public static IServiceCollection AddEmailModule(this IServiceCollection services)
    {
        services.AddScoped<IEmailService, EmailService>();

        return services;
    }
}