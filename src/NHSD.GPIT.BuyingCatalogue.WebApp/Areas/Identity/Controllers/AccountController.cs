using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Controllers
{
    [Area("Identity")]
    [Route("Identity/Account")]
    public sealed class AccountController : Controller
    {
        [HttpGet("Login")]
        public async Task Login(string returnUrl)
        {
            var action = Url.Action(nameof(HandleLogin), new { returnUrl });
            var props = new AuthenticationProperties { RedirectUri = action };

            await HttpContext.ChallengeAsync(props);
        }

        [HttpGet("login-callback")]
        public IActionResult HandleLogin(string returnUrl = null)
            => Redirect(GetLogonReturnUrl(returnUrl));

        [HttpGet("Logout")]
        public IActionResult Logout() => new SignOutResult(
            [CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme],
            new AuthenticationProperties { RedirectUri = "/" });

        private string GetLogonReturnUrl(string returnUrl)
        {
            if (!string.IsNullOrWhiteSpace(returnUrl))
                return returnUrl;

            var isAdmin = User.IsAdmin();

            return isAdmin
                ? Url.Action(
                    nameof(Admin.Controllers.HomeController.Index),
                    typeof(Admin.Controllers.HomeController).ControllerName(),
                    new { area = typeof(Admin.Controllers.HomeController).AreaName() })
                : Url.Action(
                    nameof(BuyerDashboardController.Index),
                    typeof(BuyerDashboardController).ControllerName(),
                    new { area = typeof(BuyerDashboardController).AreaName(), internalOrgId = User.GetPrimaryOrganisationInternalIdentifier() });
        }
    }
}
