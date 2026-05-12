using System.Security.Claims;
using APP.Application.Security;

namespace APP.WEB.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static bool HasPermission(this ClaimsPrincipal user, string permission)
    {
        return user.Identity?.IsAuthenticated == true
            && user.HasClaim(AppClaimTypes.Permission, permission);
    }
}
