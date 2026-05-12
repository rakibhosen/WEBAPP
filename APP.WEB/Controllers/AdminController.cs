using APP.Application.Identity;
using APP.Application.Security;
using APP.WEB.Authorization;
using APP.WEB.Extensions;
using APP.WEB.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APP.WEB.Controllers;

[Authorize(Policy = PermissionPolicyProvider.PolicyPrefix + AppPermissions.UsersManage)]
public sealed class AdminController(IIdentityStore identityStore) : Controller
{
    public async Task<IActionResult> Users(CancellationToken cancellationToken)
    {
        var users = await identityStore.GetUsersAsync(cancellationToken);
        var roles = await identityStore.GetRolesAsync(cancellationToken);
        var rolesByName = roles.ToDictionary(role => role.Name, StringComparer.OrdinalIgnoreCase);

        var model = new UserAccessViewModel(
            users
                .Select(user => new UserAccessUserViewModel(
                    user.DisplayName,
                    user.UserName,
                    user.Roles,
                    user.Roles
                        .Where(rolesByName.ContainsKey)
                        .SelectMany(role => rolesByName[role].Permissions)
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .Order(StringComparer.OrdinalIgnoreCase)
                        .ToArray()))
                .ToArray(),
            users.Count,
            roles.Count(role => role.Permissions.Contains(AppPermissions.UsersManage, StringComparer.OrdinalIgnoreCase)),
            "Permission-based");

        return this.ViewOrPartial(nameof(Users), model);
    }

    public async Task<IActionResult> UserEditor(CancellationToken cancellationToken)
    {
        var roles = await identityStore.GetRolesAsync(cancellationToken);
        return PartialView("_UserEditor", roles.OrderBy(role => role.Name).ToArray());
    }
}
