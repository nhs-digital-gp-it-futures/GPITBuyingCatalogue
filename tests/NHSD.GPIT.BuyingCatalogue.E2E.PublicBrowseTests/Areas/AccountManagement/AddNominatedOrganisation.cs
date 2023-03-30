using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common.Organisation;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.AccountManagement.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation.Organisation;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.AccountManagement
{
    [Collection(nameof(SharedContextCollection))]
    public class AddNominatedOrganisation : AccountManagerTestBase, IDisposable
    {
        private const int OrganisationId = 176;

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(OrganisationId), OrganisationId.ToString() },
        };

        public AddNominatedOrganisation(LocalWebApplicationFactory factory)
            : base(factory, typeof(ManageAccountController), nameof(ManageAccountController.AddNominatedOrganisation), Parameters)
        {
        }

        [Fact]
        public void AddNominatedOrganisation_AllElementsDisplayed()
        {
            CommonActions.ElementIsDisplayed(CommonSelectors.GoBackLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.Header1).Should().BeTrue();
            CommonActions.ElementIsDisplayed(NominatedOrganisationObjects.SelectedOrganisation).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.SubmitButton).Should().BeTrue();
        }

        [Fact]
        public void AddNominatedOrganisation_ClickGoBackLink_DisplaysCorrectPage()
        {
            CommonActions.ClickLinkElement(CommonSelectors.GoBackLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ManageAccountController),
                nameof(ManageAccountController.NominatedOrganisations)).Should().BeTrue();
        }

        [Fact]
        public void AddNominatedOrganisation_ClickContinue_DisplaysValidationErrors()
        {
            CommonActions.ClickLinkElement(CommonSelectors.SubmitButton);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ManageAccountController),
                nameof(ManageAccountController.AddNominatedOrganisation)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                NominatedOrganisationObjects.SelectedOrganisationError,
                AddNominatedOrganisationModelValidator.OrganisationMissingError).Should().BeTrue();
        }

        [Fact]
        public void AddNominatedOrganisation_FilterOrganisations_WithMatches_ExpectedResult()
        {
            var organisation = GetOrganisationNomination();

            CommonActions.ElementAddValue(NominatedOrganisationObjects.SelectedOrganisation, organisation.Name);
            CommonActions.WaitUntilElementIsDisplayed(NominatedOrganisationObjects.SearchListBox);

            CommonActions.ElementIsDisplayed(NominatedOrganisationObjects.SearchResult(0)).Should().BeTrue();
            CommonActions.ElementTextEqualTo(NominatedOrganisationObjects.SearchResult(0), organisation.Name)
                .Should()
                .BeTrue();
        }

        [Fact]
        public void AddNominatedOrganisation_FilterOrganisations_WithNoMatches_ExpectedResult()
        {
            var organisation = GetOrganisationNomination();

            CommonActions.ElementAddValue(
                NominatedOrganisationObjects.SelectedOrganisation,
                organisation.Name + "XYZ");
            CommonActions.WaitUntilElementIsDisplayed(NominatedOrganisationObjects.SearchListBox);

            CommonActions.ElementIsDisplayed(NominatedOrganisationObjects.SearchResultsErrorMessage).Should().BeTrue();
        }

        [Fact]
        public async Task AddNominatedOrganisation_SelectOrganisation_ClickContinue_ExpectedResult()
        {
            await using var context = GetEndToEndDbContext();

            var organisation = GetOrganisationNomination();

            var existingRelationships = await context.RelatedOrganisations
                .AsNoTracking()
                .Where(x => x.RelatedOrganisationId == OrganisationId)
                .ToListAsync();

            existingRelationships.Should().BeEmpty();

            CommonActions.AutoCompleteAddValue(
                NominatedOrganisationObjects.SelectedOrganisation,
                organisation.Name);
            CommonActions.ClickLinkElement(NominatedOrganisationObjects.SearchResult(0));
            CommonActions.ClickLinkElement(CommonSelectors.SubmitButton);

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(ManageAccountController),
                    nameof(ManageAccountController.NominatedOrganisations))
                .Should()
                .BeTrue();

            existingRelationships = await context.RelatedOrganisations
                .Where(x => x.RelatedOrganisationId == OrganisationId)
                .ToListAsync();

            existingRelationships.Should().ContainSingle();
        }

        private Organisation GetOrganisationNomination()
        {
            const string validOrganisationId = "CG-15F";
            using var context = GetEndToEndDbContext();

            return context.Organisations.AsNoTracking()
                .First(x => x.InternalIdentifier == validOrganisationId);
        }

        public void Dispose()
        {
            using var context = GetEndToEndDbContext();
            var relationships = context.RelatedOrganisations.Where(x => x.RelatedOrganisationId == OrganisationId).ToList();
            relationships.ForEach(x => context.RelatedOrganisations.Remove(x));
            context.SaveChanges();
        }
    }
}
