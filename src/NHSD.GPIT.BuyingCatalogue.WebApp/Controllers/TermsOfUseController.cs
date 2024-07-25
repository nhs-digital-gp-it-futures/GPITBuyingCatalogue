using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Users;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Controllers
{
    public class TermsOfUseController : Controller
    {
        private readonly IUsersService usersService;
        private readonly TermsOfUseSettings settings;

        public TermsOfUseController(
            IUsersService usersService,
            TermsOfUseSettings settings)
        {
            this.usersService = usersService ?? throw new ArgumentNullException(nameof(usersService));
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        [HttpGet("terms-of-use")]
        public async Task<IActionResult> TermsOfUse(string returnUrl = "./")
        {
            var user = await GetUserAsync();
            var userRoles = user?.AspNetUserRoles?.Select(x => x.Role.Name) ?? Enumerable.Empty<string>();
            return View(new TermsOfUseModel(user, userRoles, settings.RevisionDate)
            {
                ReturnUrl = returnUrl,
                IsAuthenticated = User.Identity.IsAuthenticated,
            });
        }

        [Authorize]
        [HttpPost("terms-of-use")]
        public async Task<IActionResult> TermsOfUse(
            TermsOfUseModel model)
        {
            if (!ModelState.IsValid)
            {
                model.IsAuthenticated = User.Identity.IsAuthenticated;
                return View(model);
            }

            if (!User.Identity?.IsAuthenticated ?? false) return RedirectToAction(nameof(TermsOfUse));

            await usersService.SetTermsOfUse(User.UserId(), model.HasOptedInUserResearch);

            return Redirect(model.ReturnUrl);
        }

        private async Task<AspNetUser> GetUserAsync()
        {
            if (!User.Identity.IsAuthenticated)
                return null;

            var userId = User.UserId();
            var user = await usersService.GetUser(userId);

            return user;
        }
    }
}
