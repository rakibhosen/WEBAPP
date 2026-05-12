using System.Security.Claims;

namespace APP.Application.Security;

public sealed class PermissionChecker : IPermissionChecker
{
    public bool HasPermission(ClaimsPrincipal user, string permission)
    {
        return user.Identity?.IsAuthenticated == true
            && user.HasClaim(AppClaimTypes.Permission, permission);
    }
}
