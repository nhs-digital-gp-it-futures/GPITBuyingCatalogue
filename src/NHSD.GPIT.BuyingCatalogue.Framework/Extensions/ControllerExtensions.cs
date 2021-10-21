using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Extensions
{
    public static class ControllerExtensions
    {
        public static async Task<string> RenderViewAsync<TModel>(
            this Controller controller,
            string viewName,
            TModel model,
            bool partial = false)
        {
            if (controller is null)
                throw new ArgumentNullException(nameof(controller));

            if (string.IsNullOrEmpty(viewName))
            {
                viewName = controller.ControllerContext.ActionDescriptor.ActionName;
            }

            controller.ViewData.Model = model;

            await using var writer = new StringWriter();

            var viewEngine = controller.HttpContext.RequestServices.GetService<ICompositeViewEngine>();
            if (viewEngine is null)
                throw new InvalidOperationException($"{nameof(ICompositeViewEngine)} service is not available");

            var viewResult = viewEngine.FindView(controller.ControllerContext, viewName, !partial);

            if (!viewResult.Success)
            {
                return $"A view with the name {viewName} could not be found";
            }

            var viewContext = new ViewContext(
                controller.ControllerContext,
                viewResult.View,
                controller.ViewData,
                controller.TempData,
                writer,
                new HtmlHelperOptions());

            await viewResult.View.RenderAsync(viewContext);

            return writer.GetStringBuilder().ToString();
        }
    }
}
