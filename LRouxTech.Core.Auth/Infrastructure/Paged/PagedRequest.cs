namespace LRouxTech.Core.Auth.Infrastructure.Paged;

public record PagedRequest(int PageIndex = 1, int PageSize = 10, string Search = "");