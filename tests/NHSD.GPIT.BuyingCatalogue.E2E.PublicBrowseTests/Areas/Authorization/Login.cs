﻿using System;
using System.Threading.Tasks;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Authorization;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Authorization
{
    public sealed class Login : AnonymousTestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        private const string EmailError = "Enter your email address";
        private const string PasswordError = "Enter your password";
        private const string EmailPasswordNotRecognisedError = "The username or password were not recognised. Please try again.";
        private const string DisabledError = "There is a problem accessing your account.";

        public Login(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(HomeController),
                  nameof(HomeController.Index),
                  null)
        {
        }

        [Fact]
        public void Login_FormDisplayed()
        {
            AuthorizationPages.LoginActions.EmailAddressInputDisplayed().Should().BeTrue();
            AuthorizationPages.LoginActions.PasswordInputDisplayed().Should().BeTrue();
            AuthorizationPages.LoginActions.LoginButtonDisplayed().Should().BeTrue();
        }

        [Fact]
        public async Task Login_LoginSuccessful()
        {
            await using var context = GetUsersContext();
            var userEmail = GetAdmin().Email;

            AuthorizationPages.LoginActions.Login(userEmail, DefaultPassword);

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(HomeController),
                    nameof(HomeController.Index))
                .Should()
                .BeTrue();

            AuthorizationPages.CommonActions.LogoutLinkDisplayed().Should().BeTrue();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(HomeController),
                nameof(HomeController.Index)).Should().BeTrue();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public async Task Login_NullOrEmptyEmail_UnsuccessfulLogin(string email)
        {
            await using var context = GetUsersContext();

            AuthorizationPages.LoginActions.Login(email, DefaultPassword);

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(AccountController),
                    nameof(AccountController.Login))
                .Should()
                .BeTrue();

            AuthorizationPages.CommonActions.LogoutLinkDisplayed().Should().BeFalse();

            AuthorizationPages.LoginActions.EmailAddressInputDisplayed().Should().BeTrue();
            AuthorizationPages.LoginActions.PasswordInputDisplayed().Should().BeTrue();
            AuthorizationPages.LoginActions.LoginButtonDisplayed().Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(AuthorizationObjects.EmailError, EmailError).Should().BeTrue();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public async Task Login_NullOrEmptyPassword_UnsuccessfulLogin(string password)
        {
            await using var context = GetUsersContext();
            var userEmail = GetAdmin().Email;

            AuthorizationPages.LoginActions.Login(userEmail, password);

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(AccountController),
                    nameof(AccountController.Login))
                .Should()
                .BeTrue();

            AuthorizationPages.CommonActions.LogoutLinkDisplayed().Should().BeFalse();

            AuthorizationPages.LoginActions.EmailAddressInputDisplayed().Should().BeTrue();
            AuthorizationPages.LoginActions.PasswordInputDisplayed().Should().BeTrue();
            AuthorizationPages.LoginActions.LoginButtonDisplayed().Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(AuthorizationObjects.PasswordError, PasswordError).Should().BeTrue();
        }

        [Theory]
        [InlineData("user", "falsePassword")]
        [InlineData("falseUser@email.com", "password")]
        public async Task Login_InvalidEmailOrPassword_UnsuccessfulLogin(string user, string password)
        {
            await using var context = GetUsersContext();
            var userEmail = user == "user" ? GetAdmin().Email : user;
            var userPassword = password == "password" ? DefaultPassword : password;

            AuthorizationPages.LoginActions.Login(userEmail, userPassword);

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(AccountController),
                    nameof(AccountController.Login))
                .Should()
                .BeTrue();

            AuthorizationPages.CommonActions.LogoutLinkDisplayed().Should().BeFalse();

            AuthorizationPages.LoginActions.EmailAddressInputDisplayed().Should().BeTrue();
            AuthorizationPages.LoginActions.PasswordInputDisplayed().Should().BeTrue();
            AuthorizationPages.LoginActions.LoginButtonDisplayed().Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(AuthorizationObjects.EmailError, EmailPasswordNotRecognisedError).Should().BeTrue();
            CommonActions.ElementShowingCorrectErrorMessage(AuthorizationObjects.PasswordError, EmailPasswordNotRecognisedError).Should().BeTrue();
        }

        [Fact]
        public async Task Login_DisabledUser_UnsuccessfulLogin()
        {
            await using var context = GetUsersContext();
            var user = GetAdmin();

            user.Disabled = true;
            context.Update(user);
            context.SaveChanges();

            AuthorizationPages.LoginActions.Login(user.Email, DefaultPassword);

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(AccountController),
                    nameof(AccountController.Login))
                .Should()
                .BeTrue();

            AuthorizationPages.CommonActions.LogoutLinkDisplayed().Should().BeFalse();

            AuthorizationPages.LoginActions.EmailAddressInputDisplayed().Should().BeTrue();
            AuthorizationPages.LoginActions.PasswordInputDisplayed().Should().BeTrue();
            AuthorizationPages.LoginActions.LoginButtonDisplayed().Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();

            CommonActions.ElementTextContains(AuthorizationObjects.DisabledError, DisabledError).Should().BeTrue();
        }

        public void Dispose()
        {
            var context = GetUsersContext();
            var user = GetAdmin();

            user.Disabled = false;
            context.Update(user);
            context.SaveChanges();

            // Force logout at end of test
            Driver.Manage().Cookies.DeleteAllCookies();
        }
    }
}
