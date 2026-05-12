using System.Security.Claims;

namespace APP.Application.Security;

public interface IPermissionChecker
{
    bool HasPermission(ClaimsPrincipal user, string permission);
}
