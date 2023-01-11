﻿using System;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
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
using NuGet.Configuration;
using Xunit;
using static NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing.Constants;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.ActionFilters
{
    public static class UpdatePasswordActionFilterTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(UpdatePasswordActionFilter).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonAutoData]
        public static async Task OnActionExecutionAsync_IdentityAccountPath_CallsNext(
            ActionExecutingContext executingContext,
            ActionExecutedContext executedContext,
            Mock<ActionExecutionDelegate> nextDelegate,
            UpdatePasswordActionFilter filter)
        {
            executingContext.HttpContext.Request.Path = "/identity/account";

            nextDelegate.Setup(d => d())
                .ReturnsAsync(executedContext);

            await filter.OnActionExecutionAsync(executingContext, nextDelegate.Object);

            nextDelegate.Verify(d => d(), Times.Once());
        }

        [Theory]
        [CommonAutoData]
        public static async Task OnActionExecutionAsync_HomeErrorPath_CallsNext(
            ActionExecutingContext executingContext,
            ActionExecutedContext executedContext,
            Mock<ActionExecutionDelegate> nextDelegate,
            UpdatePasswordActionFilter filter)
        {
            executingContext.HttpContext.Request.Path = "/home/error";

            nextDelegate.Setup(d => d())
                .ReturnsAsync(executedContext);

            await filter.OnActionExecutionAsync(executingContext, nextDelegate.Object);

            nextDelegate.Verify(d => d(), Times.Once());
        }

        [Theory]
        [CommonAutoData]
        public static async Task OnActionExecutionAsync_NotAuthenticated_CallsNext(
            ActionExecutingContext executingContext,
            ActionExecutedContext executedContext,
            Mock<ActionExecutionDelegate> nextDelegate,
            UpdatePasswordActionFilter filter)
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
        public static async Task OnActionExecutionAsync_UserIdNull_CallsNext(
            ActionExecutingContext executingContext,
            ActionExecutedContext executedContext,
            Mock<ActionExecutionDelegate> nextDelegate,
            Mock<UserManager<AspNetUser>> userManager)
        {
            var claims = new Claim[]
            {
                new Claim(CatalogueClaims.OrganisationFunction, "Authority"),
            };

            executingContext.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims, "mock"));

            userManager.Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns((string)null);

            nextDelegate.Setup(d => d())
                .ReturnsAsync(executedContext);

            var filter = new UpdatePasswordActionFilter(userManager.Object);

            await filter.OnActionExecutionAsync(executingContext, nextDelegate.Object);

            nextDelegate.Verify(d => d(), Times.Once());
        }

        [Theory]
        [CommonAutoData]
        public static async Task OnActionExecutionAsync_UserNull_CallsNext(
            ActionExecutingContext executingContext,
            ActionExecutedContext executedContext,
            Mock<ActionExecutionDelegate> nextDelegate,
            Mock<UserManager<AspNetUser>> userManager)
        {
            var userId = "1";
            var claims = new Claim[]
            {
                new Claim(CatalogueClaims.OrganisationFunction, "Authority"),
            };

            executingContext.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims, "mock"));

            userManager.Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(userId);
            userManager.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync((AspNetUser)null);

            nextDelegate.Setup(d => d())
                .ReturnsAsync(executedContext);

            var filter = new UpdatePasswordActionFilter(userManager.Object);

            await filter.OnActionExecutionAsync(executingContext, nextDelegate.Object);

            nextDelegate.Verify(d => d(), Times.Once());
        }

        [Theory]
        [CommonInlineAutoData(0)]
        [CommonInlineAutoData(364)]
        public static async Task OnActionExecutionAsync_UserPasswordNotExpired_CallsNext(
            int daysSincePasswordChange,
            ActionExecutingContext executingContext,
            ActionExecutedContext executedContext,
            Mock<ActionExecutionDelegate> nextDelegate,
            Mock<UserManager<AspNetUser>> userManager,
            AspNetUser user)
        {
            var userId = "1";
            var claims = new Claim[]
            {
                new Claim(CatalogueClaims.OrganisationFunction, "Authority"),
            };

            executingContext.HttpContext.User = new ClaimsPrincipal(
                new ClaimsIdentity(claims, "mock"));

            user.PasswordUpdatedDate = DateTime.UtcNow.AddDays(-daysSincePasswordChange);

            userManager.Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(userId);
            userManager.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync(user);

            nextDelegate.Setup(d => d())
                .ReturnsAsync(executedContext);

            var filter = new UpdatePasswordActionFilter(userManager.Object);

            await filter.OnActionExecutionAsync(executingContext, nextDelegate.Object);

            nextDelegate.Verify(d => d(), Times.Once());
        }

        [Theory]
        [CommonAutoData]
        public static async Task OnActionExecutionAsync_UserPasswordExpired_Redirects(
            ActionExecutingContext executingContext,
            ActionExecutedContext executedContext,
            Mock<ActionExecutionDelegate> nextDelegate,
            AspNetUser user,
            Mock<UserManager<AspNetUser>> userManager)
        {
            var userId = "1";
            var claims = new Claim[]
            {
                new Claim(CatalogueClaims.OrganisationFunction, "Authority"),
            };

            executingContext.HttpContext.User = new ClaimsPrincipal(
                new ClaimsIdentity(claims, "mock"));

            user.PasswordUpdatedDate = DateTime.UtcNow.AddDays(-365);

            userManager.Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(userId);
            userManager.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync(user);

            nextDelegate.Setup(d => d())
                .ReturnsAsync(executedContext);

            var filter = new UpdatePasswordActionFilter(userManager.Object);

            await filter.OnActionExecutionAsync(executingContext, nextDelegate.Object);

            nextDelegate.Verify(d => d(), Times.Never);
            var result = executingContext.Result.As<RedirectToActionResult>();
            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(AccountController.UpdatePassword));
        }
    }
}
