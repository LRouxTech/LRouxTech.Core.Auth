namespace LRouxTech.Core.Auth.Core.ViewModels.User.Request;

public record PasswordCreationRequest(string token, string password, string passwordConfirmation);