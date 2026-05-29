namespace LRouxTech.Core.Auth.Core.ViewModels.User.Request;

public record UpdatePasswordRequest(string token, string newPassword, string newPasswordConfirm);