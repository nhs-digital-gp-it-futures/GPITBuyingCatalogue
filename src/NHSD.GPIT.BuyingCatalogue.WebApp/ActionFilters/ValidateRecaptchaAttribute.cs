using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Security;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Security.Models;
using NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers.Google;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.ActionFilters;

public class ValidateRecaptchaAttribute : ActionFilterAttribute
{
    private const string RecaptchaFormKey = "g-recaptcha-response";

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var settings = context.HttpContext.RequestServices.GetRequiredService<IOptions<RecaptchaSettings>>().Value;
        if (!settings.IsEnabled)
        {
            await next();
            return;
        }

        _ = context.HttpContext.Request.Form.TryGetValue(RecaptchaFormKey, out var recaptchaResponse);
        var service = context.HttpContext.RequestServices.GetRequiredService<IRecaptchaVerificationService>();

        var isValid = await service.Validate(recaptchaResponse);
        if (!isValid)
        {
            context.ModelState.AddModelError(GoogleRecaptchaTagHelper.TagName, "Complete verification process");
        }

        await next();
    }
}
