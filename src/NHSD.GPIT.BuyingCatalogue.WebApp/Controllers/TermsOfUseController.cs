using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Controllers
{
    public class TermsOfUseController : Controller
    {
        private readonly UserManager<AspNetUser> userManager;
        private readonly TermsOfUseSettings settings;

        public TermsOfUseController(
            UserManager<AspNetUser> userManager,
            TermsOfUseSettings settings)
        {
            this.userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        [HttpGet("terms-of-use")]
        public async Task<IActionResult> TermsOfUse(string returnUrl = "./")
        {
            var user = await GetUserAsync();
            var userRoles = await GetUserRoles(user);
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

            var user = await GetUserAsync();

            if (!user.HasAcceptedLatestTermsOfUse(settings.RevisionDate))
            {
                user.AcceptedTermsOfUseDate = DateTime.UtcNow;
            }

            user.HasOptedInUserResearch = model.HasOptedInUserResearch;

            await userManager.UpdateAsync(user);

            return Redirect(model.ReturnUrl);
        }

        private async Task<AspNetUser> GetUserAsync()
        {
            if (!User.Identity.IsAuthenticated)
                return null;

            var userId = userManager.GetUserId(User);
            var user = await userManager.FindByIdAsync(userId);

            return user;
        }

        private async Task<IList<string>> GetUserRoles(AspNetUser user)
        {
            if (user == null)
                return Enumerable.Empty<string>().ToList();

            return await userManager.GetRolesAsync(user);
        }
    }
}
