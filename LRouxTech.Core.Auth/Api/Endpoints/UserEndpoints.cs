using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using LRouxTech.Core.Auth.Core.Interfaces;
using LRouxTech.Core.Auth.Core.ViewModels.User;

namespace LRouxTech.Core.Auth.Api.Endpoints;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder endpoints, string prefix = "/api/users")
    {
        var group = endpoints.MapGroup(prefix);

        // POST /api/users/login
        group.MapPost("/login", async (UserLoginRequest request, IUserService userService) =>
            {
                var user = await userService.Login(request);
                return user is not null ? Results.Ok(user) : Results.Unauthorized();
            })
            .WithName("LoginUser");

        // POST /api/users/create
        group.MapPost("/create", async (CreateUserRequest request, IUserService userService) =>
            {
                var user = await userService.Create(request);
                // Returns 201 Created. Adjust the location URI as needed for your architecture.
                return Results.Created($"{prefix}/{user.Id}", user);
            })
            .WithName("CreateUser");

        // POST /api/users/authenticate
        group.MapPost("/authenticate", async (AuthenticateUserRequest request, IUserService userService) =>
            {
                var user = await userService.Authenticate(request);
                return user is not null ? Results.Ok(user) : Results.Unauthorized();
            })
            .WithName("AuthenticateUser");

        return endpoints;
    }
}