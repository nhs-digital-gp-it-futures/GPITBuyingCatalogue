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
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.ActionFilters
{
    public static class UpdatePasswordActionFilterTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(UpdatePasswordActionFilter).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [MockAutoData]
        public static async Task OnActionExecutionAsync_IdentityAccountPath_CallsNext(
            ActionExecutingContext executingContext,
            ActionExecutedContext executedContext,
            UpdatePasswordActionFilter filter)
        {
            executingContext.HttpContext.Request.Path = "/identity/account";

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
        public static async Task OnActionExecutionAsync_HomeErrorPath_CallsNext(
            ActionExecutingContext executingContext,
            ActionExecutedContext executedContext,
            UpdatePasswordActionFilter filter)
        {
            executingContext.HttpContext.Request.Path = "/home/error";

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
        public static async Task OnActionExecutionAsync_NotAuthenticated_CallsNext(
            ActionExecutingContext executingContext,
            ActionExecutedContext executedContext,
            UpdatePasswordActionFilter filter)
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
        public static async Task OnActionExecutionAsync_UserIdNull_CallsNext(
            ActionExecutingContext executingContext,
            ActionExecutedContext executedContext,
            UserManager<AspNetUser> userManager,
            PasswordSettings passwordSettings)
        {
            passwordSettings.PasswordExpiryDays = 365;

            var claims = new Claim[]
            {
                new Claim(ClaimTypes.Role, "Authority"),
            };

            executingContext.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims, "mock"));

            userManager.GetUserId(Arg.Any<ClaimsPrincipal>()).Returns((string)null);

            var filter = new UpdatePasswordActionFilter(userManager, passwordSettings);

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
        public static async Task OnActionExecutionAsync_UserNull_CallsNext(
            ActionExecutingContext executingContext,
            ActionExecutedContext executedContext,
            UserManager<AspNetUser> userManager,
            PasswordSettings passwordSettings)
        {
            var userId = "1";
            passwordSettings.PasswordExpiryDays = 365;

            var claims = new Claim[]
            {
                new Claim(ClaimTypes.Role, "Authority"),
            };

            executingContext.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims, "mock"));

            userManager.GetUserId(Arg.Any<ClaimsPrincipal>()).Returns(userId);
            userManager.FindByIdAsync(userId).Returns((AspNetUser)null);

            var filter = new UpdatePasswordActionFilter(userManager, passwordSettings);

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
        [MockInlineAutoData(0)]
        [MockInlineAutoData(364)]
        public static async Task OnActionExecutionAsync_UserPasswordNotExpired_CallsNext(
            int daysSincePasswordChange,
            ActionExecutingContext executingContext,
            ActionExecutedContext executedContext,
            UserManager<AspNetUser> userManager,
            AspNetUser user,
            PasswordSettings passwordSettings)
        {
            var userId = "1";
            var claims = new Claim[]
            {
                new Claim(ClaimTypes.Role, "Authority"),
            };

            passwordSettings.PasswordExpiryDays = 365;

            executingContext.HttpContext.User = new ClaimsPrincipal(
                new ClaimsIdentity(claims, "mock"));

            user.PasswordUpdatedDate = DateTime.UtcNow.AddDays(-daysSincePasswordChange);

            userManager.GetUserId(Arg.Any<ClaimsPrincipal>()).Returns(userId);
            userManager.FindByIdAsync(userId).Returns(user);

            var filter = new UpdatePasswordActionFilter(userManager, passwordSettings);

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
        public static async Task OnActionExecutionAsync_UserPasswordExpired_Redirects(
            ActionExecutingContext executingContext,
            ActionExecutedContext executedContext,
            AspNetUser user,
            UserManager<AspNetUser> userManager,
            PasswordSettings passwordSettings)
        {
            var userId = "1";
            var claims = new Claim[]
            {
                new Claim(ClaimTypes.Role, "Authority"),
            };

            executingContext.HttpContext.User = new ClaimsPrincipal(
                new ClaimsIdentity(claims, "mock"));

            user.PasswordUpdatedDate = DateTime.UtcNow.AddDays(-passwordSettings.PasswordExpiryDays);

            userManager.GetUserId(Arg.Any<ClaimsPrincipal>()).Returns(userId);
            userManager.FindByIdAsync(userId).Returns(user);

            var filter = new UpdatePasswordActionFilter(userManager, passwordSettings);

            bool called = false;

            await filter.OnActionExecutionAsync(executingContext, NextDelegate);

            called.Should().BeFalse();
            var result = executingContext.Result.As<RedirectToActionResult>();
            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(AccountController.UpdatePassword));

            Task<ActionExecutedContext> NextDelegate()
            {
                called = true;
                return Task.FromResult(executedContext);
            }
        }
    }
}
