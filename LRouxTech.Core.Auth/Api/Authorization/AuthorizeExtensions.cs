using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace LRouxTech.Core.Auth.Api.Authorization;

public static class AuthorizationExtensions
{
    // The consumer passes their local permission class as a generic argument
    public static IServiceCollection AddCustomPermissions<TPermissions>(this IServiceCollection services) 
        where TPermissions : AppPermissions
    {
        services.AddSingleton<IAuthorizationHandler, PermissionHandler>();

        var permissionKeys = ExtractPermissionKeys(typeof(TPermissions));

        services.AddAuthorization(options =>
        {
            foreach (var key in permissionKeys)
            {
                options.AddPolicy(key.Value, policy =>
                    policy.Requirements.Add(new PermissionRequirement(key.Value)));
            }
        });

        return services;
    }

    private static List<PermissionKey> ExtractPermissionKeys(Type type)
    {
        var keys = new List<PermissionKey>();

        var nestedTypes = type.GetNestedTypes(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
        foreach (var nested in nestedTypes)
        {
            var fields = nested.GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(f => f.FieldType == typeof(PermissionKey));

            foreach (var field in fields)
            {
                keys.Add((PermissionKey)field.GetValue(null)!);
            }
        }

        return keys;
    }
    
    public static RouteHandlerBuilder RequirePermission(this RouteHandlerBuilder builder, PermissionKey permission)
    {
        return builder.RequireAuthorization(permission.Value);
    }

    public static RouteHandlerBuilder RequireRole(this RouteHandlerBuilder builder, string role)
    {
        return builder.RequireAuthorization(new AuthorizeAttribute { Roles = role });
    }
}