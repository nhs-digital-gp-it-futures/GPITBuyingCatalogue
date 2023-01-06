using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.ActionFilters
{
    public class UpdatePasswordActionFilter : ActionFilterAttribute
    {
        private readonly UserManager<AspNetUser> userManager;

        public UpdatePasswordActionFilter(
            UserManager<AspNetUser> userManager)
        {
            this.userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var passwordExpired = await PasswordHasExpired(context);
            if (context.HttpContext.Request.Path.StartsWithSegments("/identity/account") || context.HttpContext.Request.Path.StartsWithSegments("/home/error") || !passwordExpired)
            {
                await next();
                return;
            }

            context.Result = new RedirectToActionResult(
                nameof(AccountController.UpdatePassword),
                typeof(AccountController).ControllerName(),
                new { area = "Identity" });
        }

        private async Task<bool> PasswordHasExpired(ActionExecutingContext context)
        {
            var contextUser = context.HttpContext.User;
            if (contextUser.Identity is { IsAuthenticated: true })
            {
                var userId = userManager.GetUserId(contextUser);
                if (userId != null)
                {
                    var user = await userManager.FindByIdAsync(userId);
                    if (user != null && (DateTime.Now - user.PasswordUpdatedDate).Days >= 365)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
