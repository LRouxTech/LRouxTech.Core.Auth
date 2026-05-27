using LRouxTech.Core.BaseModel;

namespace LRouxTech.Core.Auth.Core.Entities;

public class Role : BaseModel<Role>
{
    public string Name { get; set; }
    public string Description { get; set; }
}