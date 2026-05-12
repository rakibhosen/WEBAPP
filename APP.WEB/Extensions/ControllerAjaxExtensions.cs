using Microsoft.AspNetCore.Mvc;

namespace APP.WEB.Extensions;

public static class ControllerAjaxExtensions
{
    public static IActionResult ViewOrPartial(this Controller controller, string viewName, object? model = null)
    {
        if (controller.Request.Headers.XRequestedWith == "XMLHttpRequest")
        {
            return controller.PartialView(viewName, model);
        }

        return controller.View(viewName, model);
    }
}
