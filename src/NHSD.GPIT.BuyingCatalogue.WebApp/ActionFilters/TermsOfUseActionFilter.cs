using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.ActionFilters
{
    public class TermsOfUseActionFilter : ActionFilterAttribute
    {
        private readonly UserManager<AspNetUser> userManager;
        private readonly TermsOfUseSettings settings;

        public TermsOfUseActionFilter(
            UserManager<AspNetUser> userManager,
            TermsOfUseSettings settings)
        {
            this.userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var contextUser = context.HttpContext.User;
            if (!contextUser.Identity.IsAuthenticated
                || (!contextUser.IsBuyer()
                || !context.HttpContext.Request.Path.StartsWithSegments("/order")))
            {
                await next();
                return;
            }

            var userId = userManager.GetUserId(contextUser);
            var user = await userManager.FindByIdAsync(userId);

            if (user.HasAcceptedLatestTermsOfUse(settings.RevisionDate))
            {
                await next();
                return;
            }

            context.Result = new RedirectToActionResult(
                nameof(TermsOfUseController.TermsOfUse),
                typeof(TermsOfUseController).ControllerName(),
                new { returnUrl = context.HttpContext.Request.Path });
        }
    }
}
