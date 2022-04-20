using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.Users;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.Users
{
    public class PersonalDetails : AuthorityTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private const int UserId = 2;

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(UserId), $"{UserId}" },
        };

        public PersonalDetails(LocalWebApplicationFactory factory)
            : base(factory, typeof(UsersController), nameof(UsersController.PersonalDetails), Parameters)
        {
        }

        [Fact]
        public void PersonalDetails_AllElementsDisplayed()
        {
            CommonActions.ElementIsDisplayed(CommonObjects.GoBackLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(UserObjects.FirstNameInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(UserObjects.LastNameInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(UserObjects.TelephoneNumberInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(UserObjects.EmailInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonObjects.SaveButton).Should().BeTrue();

            var user = GetUser();

            CommonActions.InputValueEqualTo(UserObjects.FirstNameInput, user.FirstName).Should().BeTrue();
            CommonActions.InputValueEqualTo(UserObjects.LastNameInput, user.LastName).Should().BeTrue();
            CommonActions.InputValueEqualTo(UserObjects.TelephoneNumberInput, user.PhoneNumber ?? string.Empty).Should().BeTrue();
            CommonActions.InputValueEqualTo(UserObjects.EmailInput, user.Email).Should().BeTrue();
        }

        [Fact]
        public void PersonalDetails_ClickGoBackLink_DisplaysCorrectPage()
        {
            CommonActions.ClickLinkElement(CommonObjects.GoBackLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(UsersController),
                nameof(UsersController.Details)).Should().BeTrue();
        }

        [Fact]
        public void PersonalDetails_ClickSave_DisplaysCorrectPage()
        {
            CommonActions.ClickLinkElement(CommonObjects.SaveButton);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(UsersController),
                nameof(UsersController.Details)).Should().BeTrue();
        }

        [Fact]
        public void PersonalDetails_BlankValues_ClickSave_DisplaysErrorMessage()
        {
            CommonActions.ElementAddValue(UserObjects.FirstNameInput, string.Empty);
            CommonActions.ElementAddValue(UserObjects.LastNameInput, string.Empty);
            CommonActions.ElementAddValue(UserObjects.EmailInput, string.Empty);

            CommonActions.ClickLinkElement(CommonObjects.SaveButton);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(UsersController),
                nameof(UsersController.PersonalDetails)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                UserObjects.FirstNameInputError,
                PersonalDetailsModelValidator.FirstNameMissingErrorMessage).Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                UserObjects.LastNameInputError,
                PersonalDetailsModelValidator.LastNameMissingErrorMessage).Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                UserObjects.EmailInputError,
                PersonalDetailsModelValidator.EmailMissingErrorMessage).Should().BeTrue();
        }

        [Fact]
        public void PersonalDetails_EmailWrongFormat_ClickSave_DisplaysErrorMessage()
        {
            CommonActions.ElementAddValue(UserObjects.EmailInput, "email address");

            CommonActions.ClickLinkElement(CommonObjects.SaveButton);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(UsersController),
                nameof(UsersController.PersonalDetails)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                UserObjects.EmailInputError,
                PersonalDetailsModelValidator.EmailWrongFormatErrorMessage).Should().BeTrue();
        }

        [Fact]
        public void PersonalDetails_EmailInUse_ClickSave_DisplaysErrorMessage()
        {
            var context = GetEndToEndDbContext();
            var user = context.AspNetUsers.FirstOrDefault(x => x.Id != UserId);

            CommonActions.ElementAddValue(UserObjects.EmailInput, user!.Email);

            CommonActions.ClickLinkElement(CommonObjects.SaveButton);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(UsersController),
                nameof(UsersController.PersonalDetails)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                UserObjects.EmailInputError,
                PersonalDetailsModelValidator.EmailInUseErrorMessage).Should().BeTrue();
        }

        private AspNetUser GetUser() => GetEndToEndDbContext().AspNetUsers.Single(x => x.Id == UserId);
    }
}
