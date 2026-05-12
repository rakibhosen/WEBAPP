using System.Security.Claims;

namespace APP.Application.Identity;

public interface IMenuService
{
    Task<IReadOnlyCollection<MenuItemDto>> GetMenuAsync(
        ClaimsPrincipal user,
        Func<string, string, string> urlFactory,
        CancellationToken cancellationToken = default);
}
