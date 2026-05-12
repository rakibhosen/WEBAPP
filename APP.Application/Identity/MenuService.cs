using System.Security.Claims;
using APP.Domain.Identity;
using APP.Application.Security;

namespace APP.Application.Identity;

public sealed class MenuService(IIdentityStore identityStore, IPermissionChecker permissionChecker) : IMenuService
{
    public async Task<IReadOnlyCollection<MenuItemDto>> GetMenuAsync(
        ClaimsPrincipal user,
        Func<string, string, string> urlFactory,
        CancellationToken cancellationToken = default)
    {
        var items = await identityStore.GetMenuItemsAsync(cancellationToken);

        var permittedItems = items
            .Where(item => item.Permission is null || permissionChecker.HasPermission(user, item.Permission))
            .OrderBy(item => item.SortOrder)
            .ToArray();

        var itemsByParent = permittedItems.ToLookup(item => item.ParentKey);

        MenuItemDto MapItem(AppMenuItem item)
        {
            var hasRoute = !string.IsNullOrWhiteSpace(item.Controller) && !string.IsNullOrWhiteSpace(item.Action);
            var children = itemsByParent[item.Key].OrderBy(child => child.SortOrder).Select(MapItem).ToArray();

            return new MenuItemDto(
                item.Key,
                item.Text,
                item.Controller,
                item.Action,
                hasRoute ? urlFactory(item.Controller!, item.Action!) : "#",
                item.Permission,
                item.Icon,
                children);
        }

        return itemsByParent[null].OrderBy(item => item.SortOrder).Select(MapItem).ToArray();
    }
}
