using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.Users;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.Users
{
    public class Organisation : AuthorityTestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        private const int UserId = 2;
        private const string NhsDigitalOrganisationName = "NHS Digital";
        private const string ValidOrganisationName = "NHS Leeds CCG";

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(UserId), $"{UserId}" },
        };

        public Organisation(LocalWebApplicationFactory factory)
            : base(factory, typeof(UsersController), nameof(UsersController.Organisation), Parameters)
        {
        }

        [Fact]
        public void Organisation_AllElementsDisplayed()
        {
            CommonActions.ElementIsDisplayed(CommonSelectors.GoBackLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.Header1).Should().BeTrue();
            CommonActions.ElementIsDisplayed(UserObjects.SelectedOrganisation).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.SubmitButton).Should().BeTrue();
        }

        [Fact]
        public void Organisation_ClickGoBackLink_DisplaysCorrectPage()
        {
            CommonActions.ClickLinkElement(CommonObjects.GoBackLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(UsersController),
                nameof(UsersController.Details)).Should().BeTrue();
        }

        [Fact]
        public void Organisation_ClickContinue_DisplaysCorrectPage()
        {
            CommonActions.ClickLinkElement(CommonSelectors.SubmitButton);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(UsersController),
                nameof(UsersController.Details)).Should().BeTrue();
        }

        [Fact]
        public void Organisation_FilterOrganisations_WithMatches_ExpectedResult()
        {
            CommonActions.AutoCompleteAddValue(UserObjects.SelectedOrganisation, ValidOrganisationName);
            CommonActions.WaitUntilElementIsDisplayed(UserObjects.AutoCompleteListBox);

            CommonActions.ElementIsDisplayed(UserObjects.AutoCompleteResult(0)).Should().BeTrue();
            CommonActions.ElementTextEqualTo(UserObjects.AutoCompleteResult(0), ValidOrganisationName).Should().BeTrue();
        }

        [Fact]
        public void Organisation_FilterOrganisations_WithNoMatches_ExpectedResult()
        {
            CommonActions.AutoCompleteAddValue(UserObjects.SelectedOrganisation, ValidOrganisationName + "XYZ");
            CommonActions.WaitUntilElementIsDisplayed(UserObjects.AutoCompleteListBox);

            CommonActions.ElementIsDisplayed(UserObjects.AutoCompleteErrorMessage).Should().BeTrue();
        }

        [Fact]
        public async Task Organisation_Admin_SelectNhsDigital_ClickContinue_ExpectedResult()
        {
            await using var context = GetEndToEndDbContext();

            var user = await context.AspNetUsers.SingleAsync(x => x.Id == UserId);

            user.PrimaryOrganisationId = context.Organisations
                .First(x => x.Name != NhsDigitalOrganisationName)
                .Id;

            await context.SaveChangesAsync();

            CommonActions.AutoCompleteAddValue(UserObjects.SelectedOrganisation, NhsDigitalOrganisationName);
            CommonActions.ClickLinkElement(UserObjects.AutoCompleteResult(0));
            CommonActions.ClickLinkElement(CommonSelectors.SubmitButton);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(UsersController),
                nameof(UsersController.Details)).Should().BeTrue();

            var organisationId = await GetOrganisationIdByName(NhsDigitalOrganisationName);
            user = await GetUser(UserId);

            user.PrimaryOrganisationId.Should().Be(organisationId);
        }

        [Fact]
        public void Organisation_Admin_SelectOtherOrganisation_ClickContinue_ThrowsError()
        {
            CommonActions.AutoCompleteAddValue(UserObjects.SelectedOrganisation, ValidOrganisationName);
            CommonActions.ClickLinkElement(UserObjects.AutoCompleteResult(0));
            CommonActions.ClickLinkElement(CommonSelectors.SubmitButton);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(UsersController),
                nameof(UsersController.Organisation)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementIsDisplayed(
                UserObjects.SelectedOrganisationError).Should().BeTrue();
        }

        [Fact]
        public async Task Organisation_Buyer_SelectOtherOrganisation_ClickContinue_ExpectedResult()
        {
            await using var context = GetEndToEndDbContext();

            var user = await context.AspNetUsers.SingleAsync(x => x.Id == UserId);

            user.OrganisationFunction = OrganisationFunction.BuyerName;
            user.PrimaryOrganisationId = context.Organisations
                .First(x => x.Name != NhsDigitalOrganisationName)
                .Id;

            await context.SaveChangesAsync();

            CommonActions.AutoCompleteAddValue(UserObjects.SelectedOrganisation, ValidOrganisationName);
            CommonActions.ClickLinkElement(UserObjects.AutoCompleteResult(0));
            CommonActions.ClickLinkElement(CommonSelectors.SubmitButton);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(UsersController),
                nameof(UsersController.Details)).Should().BeTrue();

            var organisationId = await GetOrganisationIdByName(ValidOrganisationName);
            user = await GetUser(UserId);

            user.PrimaryOrganisationId.Should().Be(organisationId);
        }

        public void Dispose()
        {
            using var context = GetEndToEndDbContext();

            var user = context.AspNetUsers.Single(x => x.Id == UserId);
            var organisation = context.Organisations.Single(x => x.Name == NhsDigitalOrganisationName);

            user.PrimaryOrganisationId = organisation.Id;
            user.OrganisationFunction = OrganisationFunction.AuthorityName;

            context.SaveChanges();
        }

        private async Task<int> GetOrganisationIdByName(string organisationName)
        {
            await using var context = GetEndToEndDbContext();

            var organisation = await context
                .Organisations
                .SingleAsync(x => x.Name == organisationName);

            return organisation.Id;
        }

        private async Task<AspNetUser> GetUser(int userId)
        {
            await using var context = GetEndToEndDbContext();

            return await context.AspNetUsers.SingleAsync(x => x.Id == userId);
        }
    }
}
