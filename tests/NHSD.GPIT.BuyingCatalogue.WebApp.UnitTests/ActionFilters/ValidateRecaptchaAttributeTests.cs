using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Moq;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Security;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Security.Models;
using NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers.Google;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.ActionFilters;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.ActionFilters;

public static class ValidateRecaptchaAttributeTests
{
    [Theory]
    [CommonAutoData]
    public static async Task OnActionExecutionAsync_RecaptchaDisabled_DoesNotAddModelState(
        RecaptchaSettings settings,
        ActionExecutingContext context,
        ActionExecutionDelegate next,
        [Frozen] Mock<IRecaptchaVerificationService> service,
        ValidateRecaptchaAttribute filter)
    {
        settings.IsEnabled = false;

        context.HttpContext.RequestServices = new ServiceCollection()
            .AddSingleton(Options.Create(settings))
            .AddSingleton(sp => service.Object)
            .BuildServiceProvider();

        await filter.OnActionExecutionAsync(context, next);

        context.ModelState.Should().NotContain(x => x.Key == GoogleRecaptchaTagHelper.TagName);
    }

    [Theory]
    [CommonAutoData]
    public static async Task OnActionExecutionAsync_Invalid_AddsModelState(
        RecaptchaSettings settings,
        ActionExecutingContext context,
        ActionExecutionDelegate next,
        [Frozen] Mock<IRecaptchaVerificationService> service,
        ValidateRecaptchaAttribute filter)
    {
        settings.IsEnabled = true;

        service.Setup(x => x.Validate(It.IsAny<string>()))
            .ReturnsAsync(false);

        context.HttpContext.Request.Form = new FormCollection(new Dictionary<string, StringValues>());
        context.HttpContext.RequestServices = new ServiceCollection()
            .AddSingleton(Options.Create(settings))
            .AddSingleton(sp => service.Object)
            .BuildServiceProvider();

        await filter.OnActionExecutionAsync(context, next);

        context.ModelState.Should().Contain(x => x.Key == GoogleRecaptchaTagHelper.TagName);
    }

    [Theory]
    [CommonAutoData]
    public static async Task OnActionExecutionAsync_Valid_DoesNotAddModelState(
        RecaptchaSettings settings,
        ActionExecutingContext context,
        ActionExecutionDelegate next,
        [Frozen] Mock<IRecaptchaVerificationService> service,
        ValidateRecaptchaAttribute filter)
    {
        settings.IsEnabled = true;

        service.Setup(x => x.Validate(It.IsAny<string>()))
            .ReturnsAsync(true);

        context.HttpContext.Request.Form = new FormCollection(new Dictionary<string, StringValues>());
        context.HttpContext.RequestServices = new ServiceCollection()
            .AddSingleton(Options.Create(settings))
            .AddSingleton(sp => service.Object)
            .BuildServiceProvider();

        await filter.OnActionExecutionAsync(context, next);

        context.ModelState.Should().NotContain(x => x.Key == GoogleRecaptchaTagHelper.TagName);
    }
}
