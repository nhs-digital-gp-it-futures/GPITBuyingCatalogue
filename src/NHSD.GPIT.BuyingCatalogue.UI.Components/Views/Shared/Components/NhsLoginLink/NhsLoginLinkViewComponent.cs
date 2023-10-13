using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.Components.NhsLoginLink
{
    public sealed class NhsLoginLinkViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(bool? isLoggedIn, string url, string text, string linkText = null)
        {
            var model = new LoginLinkModel
            {
                IsLoggedIn = isLoggedIn ?? false,
                Url = url,
                Text = text,
                LinkText = linkText ?? "Log in",
            };

            return View("LoginLink", model);
        }
    }
}
