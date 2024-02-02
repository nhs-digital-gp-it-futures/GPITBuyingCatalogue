using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.Components.NhsSearchIcon
{
    public sealed class NhsSearchIcon : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View("NhsSearchIcon");
        }
    }
}
