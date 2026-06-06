namespace LRouxTech.Core.Auth.Api.Authorization;

public readonly record struct PermissionKey
{
    public string Section { get; }
    public string Name { get; }
    public string Value => $"{Section}.{Name}";

    public PermissionKey(string section, string name)
    {
        Section = section;
        Name = name;
    }

    public static implicit operator string(PermissionKey key) => key.Value;
    public override string ToString() => Value;
}