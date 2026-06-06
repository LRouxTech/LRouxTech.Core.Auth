using LRouxTech.Core.Auth.Api.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using LRouxTech.Core.Auth.Core.Interfaces;
using LRouxTech.Core.Auth.Core.ViewModels.User;
using LRouxTech.Core.Auth.Core.ViewModels.User.Request;
using Microsoft.AspNetCore.Mvc;

namespace LRouxTech.Core.Auth.Api.Endpoints;

public static class RoleEndpoints
{
    public static IEndpointRouteBuilder MapRoleEndpoints(this IEndpointRouteBuilder endpoints, string prefix = "/api/role")
    {
        var group = endpoints.MapGroup(prefix);

        group.MapGet("/", async ([FromServices] IRoleService roleService) =>
            {
                var result = await roleService.GetList();
                if (result.IsFailure)
                {
                    return Results.BadRequest(result.Error);
                }

                return Results.Ok(result.Value);

            })
            .WithName("GetRoles")
            .RequireRole(AppRoles.Admin);

        return endpoints;
    }
}