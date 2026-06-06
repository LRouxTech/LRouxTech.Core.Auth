using LRouxTech.Core.Auth.Api.Authorization;

namespace LRouxTech.Sample.Auth;

public class LocalPermissions : AppPermissions
{
    public static class Inventory
    {
        public const string Section = "Inventory";

        public static readonly PermissionKey ViewStock = new(Section, nameof(ViewStock));
        public static readonly PermissionKey AdjustStock = new(Section, nameof(AdjustStock));
    }
}