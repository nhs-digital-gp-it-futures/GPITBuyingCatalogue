using System;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Constants;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.ActionFilters;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.ActionFilters
{
    public static class TermsOfUseActionFilterTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(TermsOfUseActionFilter).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonAutoData]
        public static async Task OnActionExecutionAsync_NotAuthenticated_CallsNext(
            ActionExecutingContext executingContext,
            ActionExecutedContext executedContext,
            Mock<ActionExecutionDelegate> nextDelegate,
            TermsOfUseActionFilter filter)
        {
            executingContext.HttpContext.User = new ClaimsPrincipal(
                new ClaimsIdentity());

            nextDelegate.Setup(d => d())
                .ReturnsAsync(executedContext);

            await filter.OnActionExecutionAsync(executingContext, nextDelegate.Object);

            nextDelegate.Verify(d => d(), Times.Once());
        }

        [Theory]
        [CommonAutoData]
        public static async Task OnActionExecutionAsync_NotBuyer_CallsNext(
            ActionExecutingContext executingContext,
            ActionExecutedContext executedContext,
            Mock<ActionExecutionDelegate> nextDelegate,
            TermsOfUseActionFilter filter)
        {
            var claims = new Claim[]
            {
                new Claim(ClaimTypes.Role, "Authority"),
            };

            executingContext.HttpContext.Request.Path = "/order";
            executingContext.HttpContext.User = new ClaimsPrincipal(
                new ClaimsIdentity(claims, "mock"));

            nextDelegate.Setup(d => d())
                .ReturnsAsync(executedContext);

            await filter.OnActionExecutionAsync(executingContext, nextDelegate.Object);

            nextDelegate.Verify(d => d(), Times.Once());
        }

        [Theory]
        [CommonAutoData]
        public static async Task OnActionExecutionAsync_NotOrderPath_CallsNext(
            ActionExecutingContext executingContext,
            ActionExecutedContext executedContext,
            Mock<ActionExecutionDelegate> nextDelegate,
            TermsOfUseActionFilter filter)
        {
            var claims = new Claim[]
            {
                new Claim(ClaimTypes.Role, "Buyer"),
            };

            executingContext.HttpContext.Request.Path = "/fake";
            executingContext.HttpContext.User = new ClaimsPrincipal(
                new ClaimsIdentity(claims, "mock"));

            nextDelegate.Setup(d => d())
                .ReturnsAsync(executedContext);

            await filter.OnActionExecutionAsync(executingContext, nextDelegate.Object);

            nextDelegate.Verify(d => d(), Times.Once());
        }

        [Theory]
        [CommonAutoData]
        public static async Task OnActionExecutionAsync_ValidUser_HasAccepted_CallsNext(
            ActionExecutingContext executingContext,
            ActionExecutedContext executedContext,
            Mock<ActionExecutionDelegate> nextDelegate,
            AspNetUser user,
            Mock<UserManager<AspNetUser>> userManager,
            TermsOfUseSettings settings)
        {
            var claims = new Claim[]
            {
                new Claim(ClaimTypes.Role, "Buyer"),
            };

            executingContext.HttpContext.Request.Path = "/order";
            executingContext.HttpContext.User = new ClaimsPrincipal(
                new ClaimsIdentity(claims, "mock"));

            settings.RevisionDate = DateTime.UtcNow.AddDays(-1);
            user.AcceptedTermsOfUseDate = DateTime.UtcNow;

            userManager.Setup(u => u.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(user);

            nextDelegate.Setup(d => d())
                .ReturnsAsync(executedContext);

            var filter = new TermsOfUseActionFilter(userManager.Object, settings);

            await filter.OnActionExecutionAsync(executingContext, nextDelegate.Object);

            nextDelegate.Verify(d => d(), Times.Once());
        }

        [Theory]
        [CommonAutoData]
        public static async Task OnActionExecutionAsync_ValidUser_NotAccepted_Redirects(
            ActionExecutingContext executingContext,
            ActionExecutedContext executedContext,
            Mock<ActionExecutionDelegate> nextDelegate,
            AspNetUser user,
            Mock<UserManager<AspNetUser>> userManager,
            TermsOfUseSettings settings)
        {
            var claims = new Claim[]
            {
                new Claim(ClaimTypes.Role, "Buyer"),
            };

            executingContext.HttpContext.Request.Path = "/order";
            executingContext.HttpContext.User = new ClaimsPrincipal(
                new ClaimsIdentity(claims, "mock"));

            settings.RevisionDate = DateTime.UtcNow;
            user.AcceptedTermsOfUseDate = DateTime.UtcNow.AddDays(-1);

            userManager.Setup(u => u.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(user);

            nextDelegate.Setup(d => d())
                .ReturnsAsync(executedContext);

            var filter = new TermsOfUseActionFilter(userManager.Object, settings);

            await filter.OnActionExecutionAsync(executingContext, nextDelegate.Object);

            nextDelegate.Verify(d => d(), Times.Never);
            var result = executingContext.Result.As<RedirectToActionResult>();
            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(TermsOfUseController.TermsOfUse));
            result.RouteValues.Should().NotBeNull();
            result.RouteValues.ContainsKey("returnUrl");
            result.RouteValues["returnUrl"].Should().Be(executingContext.HttpContext.Request.Path);
        }
    }
}
