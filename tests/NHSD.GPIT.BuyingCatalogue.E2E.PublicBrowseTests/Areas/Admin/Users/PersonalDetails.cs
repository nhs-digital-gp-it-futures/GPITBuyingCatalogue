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
            CommonActions.ElementIsDisplayed(UserObjects.EmailInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonObjects.SaveButton).Should().BeTrue();

            var user = GetUser();

            CommonActions.InputValueEqualTo(UserObjects.FirstNameInput, user.FirstName).Should().BeTrue();
            CommonActions.InputValueEqualTo(UserObjects.LastNameInput, user.LastName).Should().BeTrue();
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
        public void PersonalDetails_NoInputs_DisplaysErrorMessages()
        {
            CommonActions.ClearInputElement(UserObjects.FirstNameInput);
            CommonActions.ClearInputElement(UserObjects.LastNameInput);
            CommonActions.ClearInputElement(UserObjects.EmailInput);

            CommonActions.ClickLinkElement(CommonObjects.SaveButton);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(UsersController),
                nameof(UsersController.PersonalDetails)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementIsDisplayed(
                UserObjects.FirstNameInputError).Should().BeTrue();

            CommonActions.ElementIsDisplayed(
                UserObjects.LastNameInputError).Should().BeTrue();

            CommonActions.ElementIsDisplayed(
                UserObjects.EmailInputError).Should().BeTrue();
        }

        private AspNetUser GetUser() => GetEndToEndDbContext().AspNetUsers.Single(x => x.Id == UserId);
    }
}
