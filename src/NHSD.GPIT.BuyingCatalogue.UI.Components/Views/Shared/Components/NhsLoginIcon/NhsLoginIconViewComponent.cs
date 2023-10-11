using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.Components.NhsLoginIcon
{
    public sealed class NhsLoginIconViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View("NhsLoginIcon");
        }
    }
}
