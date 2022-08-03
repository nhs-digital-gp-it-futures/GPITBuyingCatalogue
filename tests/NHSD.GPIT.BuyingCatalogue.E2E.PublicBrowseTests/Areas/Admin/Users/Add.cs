using System;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.RandomData;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.Users;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.Users
{
    public class Add : AuthorityTestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        private const string NhsDigitalOrganisationName = "NHS Digital";
        private const string ValidEmailAddress = "a@b.com";

        public Add(LocalWebApplicationFactory factory)
            : base(factory, typeof(UsersController), nameof(UsersController.Add))
        {
        }

        [Fact]
        public void Add_AllElementsDisplayed()
        {
            CommonActions.ElementIsDisplayed(CommonSelectors.GoBackLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(UserObjects.SelectedOrganisation).Should().BeTrue();
            CommonActions.ElementIsDisplayed(UserObjects.FirstNameInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(UserObjects.LastNameInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(UserObjects.EmailInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(UserObjects.AccountTypeRadioButtons).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.SubmitButton).Should().BeTrue();
        }

        [Fact]
        public void Add_ClickGoBackLink_DisplaysCorrectPage()
        {
            CommonActions.ClickLinkElement(CommonSelectors.GoBackLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(UsersController),
                nameof(UsersController.Index)).Should().BeTrue();
        }

        [Fact]
        public void Add_InvalidModelState_DisplaysErrorMessage()
        {
            CommonActions.ClickLinkElement(CommonSelectors.SubmitButton);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(UsersController),
                nameof(UsersController.Add)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementIsDisplayed(
                UserObjects.SelectedOrganisationError).Should().BeTrue();

            CommonActions.ElementIsDisplayed(
                UserObjects.FirstNameInputError).Should().BeTrue();

            CommonActions.ElementIsDisplayed(
                UserObjects.LastNameInputError).Should().BeTrue();

            CommonActions.ElementIsDisplayed(
                UserObjects.EmailInputError).Should().BeTrue();

            CommonActions.ElementIsDisplayed(
                UserObjects.AccountTypeRadioButtonsError).Should().BeTrue();
        }

        [Fact]
        public void Add_Buyer_ClickSave_DisplaysCorrectPage()
        {
            var context = GetEndToEndDbContext();

            var organisationName = context.Organisations
                .First(x => x.Name != NhsDigitalOrganisationName)
                .Name;

            CommonActions.AutoCompleteAddValue(UserObjects.SelectedOrganisation, organisationName);
            CommonActions.ClickLinkElement(UserObjects.AutoCompleteResult(0));
            CommonActions.ElementAddValue(UserObjects.FirstNameInput, Strings.RandomString(10));
            CommonActions.ElementAddValue(UserObjects.LastNameInput, Strings.RandomString(10));
            CommonActions.ElementAddValue(UserObjects.EmailInput, ValidEmailAddress);
            CommonActions.ClickRadioButtonWithText("Buyer");

            CommonActions.ClickLinkElement(CommonSelectors.SubmitButton);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(UsersController),
                nameof(UsersController.Index)).Should().BeTrue();
        }

        [Fact]
        public void Add_Admin_InNhsDigital_ClickSave_DisplaysCorrectPage()
        {
            CommonActions.AutoCompleteAddValue(UserObjects.SelectedOrganisation, NhsDigitalOrganisationName);
            CommonActions.ClickLinkElement(UserObjects.AutoCompleteResult(0));
            CommonActions.ElementAddValue(UserObjects.FirstNameInput, Strings.RandomString(10));
            CommonActions.ElementAddValue(UserObjects.LastNameInput, Strings.RandomString(10));
            CommonActions.ElementAddValue(UserObjects.EmailInput, ValidEmailAddress);
            CommonActions.ClickRadioButtonWithText("Admin");

            CommonActions.ClickLinkElement(CommonSelectors.SubmitButton);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(UsersController),
                nameof(UsersController.Index)).Should().BeTrue();
        }

        public void Dispose()
        {
            var context = GetEndToEndDbContext();
            var users = context.AspNetUsers.Where(x => x.Email == ValidEmailAddress).ToList();
            context.AspNetUsers.RemoveRange(users);
            context.SaveChanges();
        }
    }
}
