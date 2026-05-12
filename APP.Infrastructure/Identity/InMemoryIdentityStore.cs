using APP.Application.Identity;
using APP.Application.Security;
using APP.Domain.Identity;

namespace APP.Infrastructure.Identity;

public sealed class InMemoryIdentityStore : IIdentityStore
{
    private static readonly AppUser[] Users =
    [
        new("admin", "System Admin", "admin123", ["Admin"]),
        new("manager", "Manager User", "manager123", ["Manager"]),
        new("user", "Standard User", "user123", ["User"])
    ];

    private static readonly AppRole[] Roles =
    [
        new("Admin", AppPermissions.All),
        new("Manager", [AppPermissions.DashboardView, AppPermissions.ReportsView]),
        new("User", [AppPermissions.DashboardView])
    ];

    private static readonly AppMenuItem[] MenuItems =
    [
        new("dashboard", "Dashboard", "Home", "Index", AppPermissions.DashboardView, 10, "grid"),
        new("banking", "Banking Operations", null, null, null, 20, "landmark"),
        new("banking-reports", "Reports", "Reports", "Index", AppPermissions.ReportsView, 10, "chart", "banking"),
        new("banking-compliance", "Compliance", null, null, null, 20, "shield", "banking"),
        new("privacy", "Privacy", "Home", "Privacy", AppPermissions.PrivacyView, 10, "shield", "banking-compliance"),
        new("administration", "Administration", null, null, null, 30, "settings"),
        new("user-access", "User Access", "Admin", "Users", AppPermissions.UsersManage, 10, "users", "administration")
    ];

    public Task<AppUser?> FindUserAsync(string userName, CancellationToken cancellationToken = default)
    {
        var user = Users.FirstOrDefault(item =>
            StringComparer.OrdinalIgnoreCase.Equals(item.UserName, userName));

        return Task.FromResult(user);
    }

    public Task<IReadOnlyCollection<AppRole>> GetRolesAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyCollection<AppRole>>(Roles);
    }

    public Task<IReadOnlyCollection<AppUser>> GetUsersAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyCollection<AppUser>>(Users);
    }

    public Task<IReadOnlyCollection<AppMenuItem>> GetMenuItemsAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyCollection<AppMenuItem>>(MenuItems);
    }
}
