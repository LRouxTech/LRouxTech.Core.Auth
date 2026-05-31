using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using LRouxTech.Core.Auth.Core.Interfaces;
using LRouxTech.Core.Auth.Core.ViewModels.User;
using LRouxTech.Core.Auth.Core.ViewModels.User.Request;

namespace LRouxTech.Core.Auth.Api.Endpoints;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder endpoints, string prefix = "/api/user")
    {
        var group = endpoints.MapGroup(prefix);

        group.MapPost("/login", async (UserLoginRequest request, IUserService userService) =>
            {
                var result = await userService.Login(request);
                if (result.IsFailure)
                {
                    return Results.BadRequest(result.Error);
                }
                return Results.Ok(result.Value);
            })
            .WithName("LoginUser");
        
        group.MapPost("/logout", async (UserLogoutRequest request, IUserService userService) =>
            {
                var result = await userService.Logout(request);
                if (result.IsFailure)
                {
                    return Results.BadRequest(result.Error);
                }
                return Results.Ok(result.Value);
            })
            .WithName("LogoutUser");

        group.MapPost("/create", async (CreateUserRequest request, IUserService userService) =>
            {
                var result = await userService.Create(request);
                if (result.IsFailure)
                {
                    return Results.BadRequest(result.Error);
                }
                return Results.Created($"{prefix}/{result.Value.Id}", result.Value);
            })
            .WithName("CreateUser");
        
        group.MapPost("/update", async (UpdateUserRequest request, IUserService userService) =>
            {
                var result = await userService.Update(request);
                if (result.IsFailure)
                {
                    return Results.BadRequest(result.Error);
                }
                return Results.Created($"{prefix}/{result.Value.Id}", result.Value);
            })
            .WithName("UpdateUser");

        group.MapPost("/authenticate", async (AuthenticateUserRequest request, IUserService userService) =>
            {
                var result = await userService.Authenticate(request);
                if (result.IsFailure)
                {
                    return Results.BadRequest(result.Error);
                }
                return Results.Ok(result.Value);
            })
            .WithName("AuthenticateUser");
        
        group.MapGet("/", async (UserListRequest request, IUserService userService) =>
            {
                var result = await userService.GetUserList(request);
                if (result.IsFailure)
                {
                    return Results.BadRequest(result.Error);
                }
                return Results.Ok(result.Value);
            })
            .WithName("GetUsers");
        
        group.MapGet("/{UserId}", async (UserDetailRequest request, IUserService userService) =>
            {
                var result = await userService.GetUser(request);
                if (result.IsFailure)
                {
                    return Results.BadRequest(result.Error);
                }
                return Results.Ok(result.Value);
            })
            .WithName("GetUser");
                
        group.MapPost("/set-password", async (PasswordCreationRequest request, IUserService userService) =>
            {
                var result = await userService.InitialPasswordSet(request);
                if (result.IsFailure)
                {
                    return Results.BadRequest(result.Error);
                }
                return Results.Ok(result.Value);
            })
            .WithName("SetUserPassword");
        
        group.MapPost("/update-password", async (UpdatePasswordRequest request, IUserService userService) =>
            {
                var result = await userService.UpdatePassword(request);
                if (result.IsFailure)
                {
                    return Results.BadRequest(result.Error);
                }
                return Results.Ok(result.Value);
            })
            .WithName("UpdateUserPassword");
        
        group.MapPost("/reset-password", async (ResetPasswordRequest request, IUserService userService) =>
            {
                var result = await userService.ResetPassword(request);
                if (result.IsFailure)
                {
                    return Results.BadRequest(result.Error);
                }
                return Results.Ok(result.Value);
            })
            .WithName("ResetUserPassword");
        
        group.MapPost("/archive/{userId}", async (ArchiveUserRequest request, IUserService userService, IHttpCurrentUserContext currentUserContext) =>
            {
                var result = await userService.ArchiveUser(request, currentUserContext.UserId!.Value);
                if (result.IsFailure)
                {
                    return Results.BadRequest(result.Error);
                }
                return Results.Ok(result.Value);
            })
            .WithName("ArchiveUser");
        
        group.MapPost("/delete/{userId}", async (DeleteUserRequest request, IUserService userService) =>
            {
                var result = await userService.DeleteUser(request);
                if (result.IsFailure)
                {
                    return Results.BadRequest(result.Error);
                }
                return Results.Ok(result.Value);
            })
            .WithName("DeleteUser");

        return endpoints;
    }
}