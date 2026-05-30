using LRouxTech.Core.ValidationResult;

namespace LRouxTech.Core.Auth.Infrastructure.Errors;

public static class SettingsErrors
{
    public static readonly Error SettingsNotFound = new(
        "Settings.NotFound",
        "One or more settings could not be found.");
}