using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common.Organisation;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.Organisation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation.Organisation;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.Organisations
{
    public class AddNominatedOrganisation : AuthorityTestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        private const int OrganisationId = 1;
        private const string ValidOrganisationName = "NHS Leeds CCG";

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(OrganisationId), OrganisationId.ToString() },
        };

        public AddNominatedOrganisation(LocalWebApplicationFactory factory)
            : base(factory, typeof(OrganisationsController), nameof(OrganisationsController.AddNominatedOrganisation), Parameters)
        {
        }

        [Fact]
        public void AddNominatedOrganisation_AllElementsDisplayed()
        {
            CommonActions.ElementIsDisplayed(CommonObjects.GoBackLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.Header1).Should().BeTrue();
            CommonActions.ElementIsDisplayed(NominatedOrganisationObjects.SelectedOrganisation).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.SubmitButton).Should().BeTrue();
        }

        [Fact]
        public void AddNominatedOrganisation_ClickGoBackLink_DisplaysCorrectPage()
        {
            CommonActions.ClickLinkElement(CommonObjects.GoBackLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrganisationsController),
                nameof(OrganisationsController.NominatedOrganisations)).Should().BeTrue();
        }

        [Fact]
        public void AddNominatedOrganisation_ClickContinue_DisplaysValidationErrors()
        {
            CommonActions.ClickLinkElement(CommonSelectors.SubmitButton);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrganisationsController),
                nameof(OrganisationsController.AddNominatedOrganisation)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                NominatedOrganisationObjects.SelectedOrganisationError,
                AddNominatedOrganisationModelValidator.OrganisationMissingError).Should().BeTrue();
        }

        [Fact]
        public void AddNominatedOrganisation_FilterOrganisations_WithMatches_ExpectedResult()
        {
            CommonActions.ElementAddValue(NominatedOrganisationObjects.SelectedOrganisation, ValidOrganisationName);
            CommonActions.WaitUntilElementIsDisplayed(NominatedOrganisationObjects.SearchListBox);

            CommonActions.ElementIsDisplayed(NominatedOrganisationObjects.SearchResult(0)).Should().BeTrue();
            CommonActions.ElementTextEqualTo(NominatedOrganisationObjects.SearchResult(0), ValidOrganisationName).Should().BeTrue();
        }

        [Fact]
        public void AddNominatedOrganisation_FilterOrganisations_WithNoMatches_ExpectedResult()
        {
            CommonActions.ElementAddValue(NominatedOrganisationObjects.SelectedOrganisation, ValidOrganisationName + "XYZ");
            CommonActions.WaitUntilElementIsDisplayed(NominatedOrganisationObjects.SearchListBox);

            CommonActions.ElementIsDisplayed(NominatedOrganisationObjects.SearchResultsErrorMessage).Should().BeTrue();
        }

        [Fact]
        public async Task AddNominatedOrganisation_SelectOrganisation_ClickContinue_ExpectedResult()
        {
            await using var context = GetEndToEndDbContext();

            var existingRelationships = await context.RelatedOrganisations
                .Where(x => x.RelatedOrganisationId == OrganisationId)
                .ToListAsync();

            existingRelationships.Should().BeEmpty();

            CommonActions.AutoCompleteAddValue(NominatedOrganisationObjects.SelectedOrganisation, ValidOrganisationName);
            CommonActions.ClickLinkElement(NominatedOrganisationObjects.SearchResult(0));
            CommonActions.ClickLinkElement(CommonSelectors.SubmitButton);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrganisationsController),
                nameof(OrganisationsController.NominatedOrganisations)).Should().BeTrue();

            existingRelationships = await context.RelatedOrganisations
                .Where(x => x.RelatedOrganisationId == OrganisationId)
                .ToListAsync();

            existingRelationships.Should().ContainSingle();
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
