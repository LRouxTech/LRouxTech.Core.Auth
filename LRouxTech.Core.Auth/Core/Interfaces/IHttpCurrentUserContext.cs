namespace LRouxTech.Core.Auth.Core.Interfaces;

public interface IHttpCurrentUserContext
{
    Guid? UserId { get; }
}