namespace LRouxTech.Core.Auth.Core.ViewModels.Permission;

public record PermissionListResponse(List<PermissionItem> permissionItems);

public record PermissionItem(Guid Id, string Section, string PermissionName);