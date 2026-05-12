using System.Diagnostics;
using APP.Application.Security;
using APP.WEB.Authorization;
using APP.WEB.Extensions;
using APP.WEB.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APP.WEB.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        [Authorize(Policy = PermissionPolicyProvider.PolicyPrefix + AppPermissions.DashboardView)]
        public IActionResult Index()
        {
            return this.ViewOrPartial(nameof(Index));
        }

        [Authorize(Policy = PermissionPolicyProvider.PolicyPrefix + AppPermissions.PrivacyView)]
        public IActionResult Privacy()
        {
            return this.ViewOrPartial(nameof(Privacy));
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
