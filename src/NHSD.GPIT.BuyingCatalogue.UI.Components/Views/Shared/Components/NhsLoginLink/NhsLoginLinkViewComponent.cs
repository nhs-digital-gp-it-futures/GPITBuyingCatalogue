using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.Components.NhsLoginLink
{
    public sealed class NhsLoginLinkViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(bool? isLoggedIn, string url, string loggedInUrl, string loggedInText, string text = null)
        {
            var model = new LoginLinkModel
            {
                IsLoggedIn = isLoggedIn ?? false,
                Url = url,
                LoggedInUrl = loggedInUrl,
                Text = text ?? "Log in",
                LoggedInText = loggedInText,
            };

            return View("LoginLink", model);
        }
    }
}
