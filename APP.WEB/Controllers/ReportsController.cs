using APP.Application.Security;
using APP.WEB.Authorization;
using APP.WEB.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APP.WEB.Controllers;

[Authorize(Policy = PermissionPolicyProvider.PolicyPrefix + AppPermissions.ReportsView)]
public sealed class ReportsController : Controller
{
    public IActionResult Index()
    {
        return this.ViewOrPartial(nameof(Index));
    }
}
