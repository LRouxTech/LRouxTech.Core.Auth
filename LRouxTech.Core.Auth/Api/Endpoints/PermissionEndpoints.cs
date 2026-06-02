using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using LRouxTech.Core.Auth.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LRouxTech.Core.Auth.Api.Endpoints;

public static class PermissionEndpoints
{
    public static IEndpointRouteBuilder MapPermissionEndpoints(this IEndpointRouteBuilder endpoints, string prefix = "/api/permission")
    {
        var group = endpoints.MapGroup(prefix);

        group.MapGet("/", async ([FromServices] IPermissionService permissionService) =>
            {
                var result = await permissionService.GetList();
                if (result.IsFailure)
                {
                    Results.BadRequest(result.Error);
                }

                return Results.Ok(result.Value);

            })
            .WithName("GetPermissions");

        return endpoints;
    }
}