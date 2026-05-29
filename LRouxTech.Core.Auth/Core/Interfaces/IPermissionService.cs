using LRouxTech.Core.Auth.Core.ViewModels.Permission;
using LRouxTech.Core.Auth.Core.ViewModels.Role;
using LRouxTech.Core.ValidationResult;

namespace LRouxTech.Core.Auth.Core.Interfaces;

public interface IPermissionService
{
    Task<Result<PermissionListResponse>> GetList();
}