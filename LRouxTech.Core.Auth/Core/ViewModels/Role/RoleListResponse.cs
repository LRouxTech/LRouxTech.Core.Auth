namespace LRouxTech.Core.Auth.Core.ViewModels.Role;

public record RoleListResponse(List<RoleItem> roleItems);

public record RoleItem(Guid Id, string Name, string Description, List<Guid> PermissionIds);