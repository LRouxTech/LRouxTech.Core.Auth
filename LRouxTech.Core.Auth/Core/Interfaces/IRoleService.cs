using LRouxTech.Core.Auth.Core.ViewModels.Role;
using LRouxTech.Core.ValidationResult;

namespace LRouxTech.Core.Auth.Core.Interfaces;

public interface IRoleService
{
    Task<Result<RoleListResponse>> GetList();
}