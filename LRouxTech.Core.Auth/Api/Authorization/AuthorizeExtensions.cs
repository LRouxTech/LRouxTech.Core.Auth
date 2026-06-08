using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace LRouxTech.Core.Auth.Api.Authorization;

public static class AuthorizationExtensions
{
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
        var targetAssembly = type.Assembly;

        var baseAssembly = typeof(AppPermissions).Assembly;

        var assembliesToScan = new[] { targetAssembly, baseAssembly }.Distinct();

        var keys = new List<PermissionKey>();

        foreach (var assembly in assembliesToScan)
        {
            var foundKeys = assembly.GetTypes()
                .SelectMany(t => t.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy))
                .Where(f => f.FieldType == typeof(PermissionKey))
                .Select(f => (PermissionKey)f.GetValue(null)!)
                .ToList();

            keys.AddRange(foundKeys);
        }

        return keys.GroupBy(k => k.Value).Select(g => g.First()).ToList();
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