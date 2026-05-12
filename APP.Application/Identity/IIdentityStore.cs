using APP.Domain.Identity;

namespace APP.Application.Identity;

public interface IIdentityStore
{
    Task<AppUser?> FindUserAsync(string userName, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<AppUser>> GetUsersAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<AppRole>> GetRolesAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<AppMenuItem>> GetMenuItemsAsync(CancellationToken cancellationToken = default);
}
