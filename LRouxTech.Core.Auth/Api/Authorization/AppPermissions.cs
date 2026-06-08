namespace LRouxTech.Core.Auth.Api.Authorization;

public class AppPermissions 
{
    public static class UserManagement
    {
        public static readonly PermissionKey Create = UserManagementSystem.Create;
        public static readonly PermissionKey Read = UserManagementSystem.Read;
        public static readonly PermissionKey Update = UserManagementSystem.Update;
        public static readonly PermissionKey Delete = UserManagementSystem.Delete;
    }
}

public static class UserManagementSystem
{
    public const string Section = "UserManagement";

    public static readonly PermissionKey Create = new(Section, nameof(Create));
    public static readonly PermissionKey Read = new(Section, nameof(Read));
    public static readonly PermissionKey Update = new(Section, nameof(Update));
    public static readonly PermissionKey Delete = new(Section, nameof(Delete));
}

