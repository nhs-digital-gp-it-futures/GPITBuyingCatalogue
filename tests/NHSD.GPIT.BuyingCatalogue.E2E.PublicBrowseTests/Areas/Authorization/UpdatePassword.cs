using System;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Authorization;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Identity;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Validators.Registration;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Authorization
{
    [Collection(nameof(SharedContextCollection))]
    public sealed class UpdatePassword : BuyerTestBase, IDisposable
    {
        private const string ValidPassword = "Pass123$$$$$$";
        private const string InvalidPassword = "Blah";

        public UpdatePassword(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(AccountController),
                  nameof(AccountController.UpdatePassword),
                  null)
        {
        }

        [Fact]
        public void UpdatePassword_FormDisplayed()
        {
            CommonActions.PageTitle().Should().BeEquivalentTo("Update Password".FormatForComparison());
            AuthorizationPages.UpdatePasswordActions.CurrentPasswordInputDisplayed();
            AuthorizationPages.UpdatePasswordActions.NewPasswordInputDisplayed();
            AuthorizationPages.UpdatePasswordActions.ConfirmPasswordInputDisplayed();
            AuthorizationPages.UpdatePasswordActions.SavePasswordButtonDisplayed();
        }

        [Fact]
        public void UpdatePassword_EmptyFields_UnsuccessfulUpdate()
        {
            AuthorizationPages.UpdatePasswordActions.ClickSavePassword();

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(AccountController),
                    nameof(AccountController.UpdatePassword))
                .Should()
                .BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(UpdatePasswordObjects.CurrentPasswordError, UpdatePasswordViewModelValidator.CurrentPasswordRequired).Should().BeTrue();
            CommonActions.ElementShowingCorrectErrorMessage(UpdatePasswordObjects.NewPasswordError, UpdatePasswordViewModelValidator.NewPasswordRequired).Should().BeTrue();
            CommonActions.ElementShowingCorrectErrorMessage(UpdatePasswordObjects.ConfirmPasswordError, UpdatePasswordViewModelValidator.ConfirmPasswordRequired).Should().BeTrue();
        }

        [Fact]
        public void UpdatePassword_NewAndConfirmNotMatched_UnsuccessfulUpdate()
        {
            AuthorizationPages.UpdatePasswordActions.SavePassword(DefaultPassword, ValidPassword, InvalidPassword);

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(AccountController),
                    nameof(AccountController.UpdatePassword))
                .Should()
                .BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(UpdatePasswordObjects.ConfirmPasswordError, UpdatePasswordViewModelValidator.ConfirmPasswordMismatch).Should().BeTrue();
        }

        [Fact]
        public void UpdatePassword_IncorrectCurrentPassword_UnsuccessfulUpdate()
        {
            AuthorizationPages.UpdatePasswordActions.SavePassword(InvalidPassword, ValidPassword, ValidPassword);

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(AccountController),
                    nameof(AccountController.UpdatePassword))
                .Should()
                .BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(UpdatePasswordObjects.CurrentPasswordError, UpdatePasswordViewModelValidator.CurrentPasswordIncorrect).Should().BeTrue();
        }

        [Fact]
        public void UpdatePassword_InvalidNewPassword_UnsuccessfulUpdate()
        {
            AuthorizationPages.UpdatePasswordActions.SavePassword(DefaultPassword, InvalidPassword, InvalidPassword);

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(AccountController),
                    nameof(AccountController.UpdatePassword))
                .Should()
                .BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(UpdatePasswordObjects.NewPasswordError, PasswordValidator.PasswordConditionsNotMet).Should().BeTrue();
        }

        [Fact]
        public void UpdatePassword_ValidPassword_SuccessfulUpdate()
        {
            AuthorizationPages.UpdatePasswordActions.SavePassword(DefaultPassword, ValidPassword, ValidPassword);

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(AccountController),
                    nameof(AccountController.Login))
                .Should()
                .BeTrue();

            var userEmail = GetBuyer().Email;
            AuthorizationPages.LoginActions.Login(userEmail, ValidPassword);

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(HomeController),
                    nameof(HomeController.Index))
                .Should()
                .BeTrue();

            AuthorizationPages.CommonActions.LogoutLinkDisplayed().Should().BeTrue();
        }

        public void Dispose()
        {
            var context = GetUsersContext();
            var user = GetBuyer();

            user.PasswordHash = new PasswordHasher<AspNetUser>().HashPassword(user, DefaultPassword);

            context.Update(user);
            context.SaveChanges();
        }
    }
}
