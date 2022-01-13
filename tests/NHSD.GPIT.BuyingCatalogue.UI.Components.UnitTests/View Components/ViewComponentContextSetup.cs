using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewComponents;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.UnitTests.View_Components
{
    internal static class ViewComponentContextSetup
    {
        internal static ViewComponentContext GetViewComponentContext()
        {
            var viewContext = new ViewContext()
            {
                HttpContext = new DefaultHttpContext(),
            };

            var viewComponentContext = new ViewComponentContext()
            {
                ViewContext = viewContext,
            };

            return viewComponentContext;
        }
    }
}
