using LRouxTech.Core.Auth.Api.Authorization;

namespace LRouxTech.Sample.Auth;

public class LocalPermissions : AppPermissions
{
    public new static class UserManagement
    {
        public static readonly PermissionKey Create = UserManagementSystem.Create;
        public static readonly PermissionKey Read = UserManagementSystem.Read;
        public static readonly PermissionKey Update = UserManagementSystem.Update;
        public static readonly PermissionKey Delete = UserManagementSystem.Delete;
    }

    public static class Inventory
    {
        public const string Section = "Inventory";

        public static readonly PermissionKey ViewStock = new(Section, nameof(ViewStock));
        public static readonly PermissionKey AdjustStock = new(Section, nameof(AdjustStock));
    }
}