using LRouxTech.Core.Auth.Api.Authorization;

namespace LRouxTech.Sample.Auth;

public class LocalRoles : AppRoles
{
    public const string InventoryAuditor = "InventoryAuditor";
    public new const string Admin = UserManagementRoles.Admin;
}