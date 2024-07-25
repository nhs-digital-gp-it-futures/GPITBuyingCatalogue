using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.ActionFilters
{
    public class TermsOfUseActionFilter : ActionFilterAttribute
    {
        private readonly BuyingCatalogueDbContext dbContext;
        private readonly TermsOfUseSettings settings;

        public TermsOfUseActionFilter(
            BuyingCatalogueDbContext dbContext,
            TermsOfUseSettings settings)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
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

            var user = await dbContext.AspNetUsers.FirstOrDefaultAsync(x => x.Id == context.HttpContext.User.UserId());
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
