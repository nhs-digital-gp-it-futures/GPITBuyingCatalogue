using System;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Idioms;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.WebApp.ActionFilters;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.ActionFilters
{
    public static class TermsOfUseActionFilterTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(TermsOfUseActionFilter).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [MockAutoData]
        public static async Task OnActionExecutionAsync_NotAuthenticated_CallsNext(
            ActionExecutingContext executingContext,
            ActionExecutedContext executedContext,
            TermsOfUseActionFilter filter)
        {
            executingContext.HttpContext.User = new ClaimsPrincipal(
                new ClaimsIdentity());

            bool called = false;

            await filter.OnActionExecutionAsync(executingContext, NextDelegate);

            called.Should().BeTrue();

            Task<ActionExecutedContext> NextDelegate()
            {
                called = true;
                return Task.FromResult(executedContext);
            }
        }

        [Theory]
        [MockAutoData]
        public static async Task OnActionExecutionAsync_NotBuyer_CallsNext(
            ActionExecutingContext executingContext,
            ActionExecutedContext executedContext,
            TermsOfUseActionFilter filter)
        {
            var claims = new Claim[]
            {
                new Claim(ClaimTypes.Role, "Authority"),
            };

            executingContext.HttpContext.Request.Path = "/order";
            executingContext.HttpContext.User = new ClaimsPrincipal(
                new ClaimsIdentity(claims, "mock"));

            bool called = false;

            await filter.OnActionExecutionAsync(executingContext, NextDelegate);

            called.Should().BeTrue();

            Task<ActionExecutedContext> NextDelegate()
            {
                called = true;
                return Task.FromResult(executedContext);
            }
        }

        [Theory]
        [MockAutoData]
        public static async Task OnActionExecutionAsync_NotOrderPath_CallsNext(
            ActionExecutingContext executingContext,
            ActionExecutedContext executedContext,
            TermsOfUseActionFilter filter)
        {
            var claims = new Claim[]
            {
                new Claim(ClaimTypes.Role, "Buyer"),
            };

            executingContext.HttpContext.Request.Path = "/fake";
            executingContext.HttpContext.User = new ClaimsPrincipal(
                new ClaimsIdentity(claims, "mock"));

            bool called = false;

            await filter.OnActionExecutionAsync(executingContext, NextDelegate);

            called.Should().BeTrue();

            Task<ActionExecutedContext> NextDelegate()
            {
                called = true;
                return Task.FromResult(executedContext);
            }
        }

        [Theory]
        [MockAutoData]
        public static async Task OnActionExecutionAsync_ValidUser_HasAccepted_CallsNext(
            ActionExecutingContext executingContext,
            ActionExecutedContext executedContext,
            AspNetUser user,
            UserManager<AspNetUser> userManager,
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

            userManager.FindByIdAsync(Arg.Any<string>())
                .Returns(user);

            var filter = new TermsOfUseActionFilter(userManager, settings);

            bool called = false;

            await filter.OnActionExecutionAsync(executingContext, NextDelegate);

            called.Should().BeTrue();

            Task<ActionExecutedContext> NextDelegate()
            {
                called = true;
                return Task.FromResult(executedContext);
            }
        }

        [Theory]
        [MockAutoData]
        public static async Task OnActionExecutionAsync_ValidUser_NotAccepted_Redirects(
            ActionExecutingContext executingContext,
            ActionExecutedContext executedContext,
            AspNetUser user,
            UserManager<AspNetUser> userManager,
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

            userManager.FindByIdAsync(Arg.Any<string>())
                .Returns(user);

            var filter = new TermsOfUseActionFilter(userManager, settings);

            bool called = false;

            await filter.OnActionExecutionAsync(executingContext, NextDelegate);

            called.Should().BeFalse();
            var result = executingContext.Result.As<RedirectToActionResult>();
            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(TermsOfUseController.TermsOfUse));
            result.RouteValues.Should().NotBeNull();
            result.RouteValues.ContainsKey("returnUrl");
            result.RouteValues["returnUrl"].Should().Be(executingContext.HttpContext.Request.Path);

            Task<ActionExecutedContext> NextDelegate()
            {
                called = true;
                return Task.FromResult(executedContext);
            }
        }
    }
}
