using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.ActionFilters;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.ActionFilters
{
    public static class BadRequestActionFilterTests
    {
        [Theory]
        [MockAutoData]
        public static void OnActionExecuted_SuccessfulResult_DoesNotRedirect(
            ActionExecutedContext context,
            BadRequestActionFilter filter)
        {
            var value = "some-response";
            context.Result = new OkObjectResult(value);

            filter.OnActionExecuted(context);

            context.Result.As<OkObjectResult>().Should().NotBeNull();
            context.Result.As<OkObjectResult>().Value.Should().Be(value);
        }

        [Theory]
        [MockAutoData]
        public static void OnActionExecuted_BadRequestResult_Redirects(
            ActionExecutedContext context,
            BadRequestActionFilter filter)
        {
            context.Result = new BadRequestObjectResult("some-error");

            filter.OnActionExecuted(context);

            context.Result.As<RedirectToActionResult>().Should().NotBeNull();
        }
    }
}
