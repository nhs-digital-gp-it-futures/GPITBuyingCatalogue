using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common.Organisation;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.Organisations
{
    [Collection(nameof(AdminCollection))]
    public class RemoveRelatedOrganisation : AuthorityTestBase, IDisposable
    {
        private const int OrganisationId = 2;
        private const int RelatedOrganisationId = 3;

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(OrganisationId), OrganisationId.ToString() },
            { nameof(RelatedOrganisationId), RelatedOrganisationId.ToString() },
        };

        public RemoveRelatedOrganisation(LocalWebApplicationFactory factory)
            : base(factory, typeof(OrganisationsController), nameof(OrganisationsController.RemoveRelatedOrganisation), Parameters)
        {
        }

        [Fact]
        public async Task RemoveRelatedOrganisation_AllElementsDisplayed()
        {
            await AddRelatedOrganisation();

            CommonActions.ElementIsDisplayed(CommonObjects.GoBackLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.Header1).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.SubmitButton).Should().BeTrue();
            CommonActions.ElementIsDisplayed(RelatedOrganisationObjects.CancelLink).Should().BeTrue();
        }

        [Fact]
        public async Task RemoveRelatedOrganisation_ClickGoBackLink_DisplaysCorrectPage()
        {
            await AddRelatedOrganisation();

            CommonActions.ClickLinkElement(CommonObjects.GoBackLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrganisationsController),
                nameof(OrganisationsController.RelatedOrganisations)).Should().BeTrue();
        }

        [Fact]
        public async Task RemoveRelatedOrganisation_ClickCancelLink_DisplaysCorrectPage()
        {
            await AddRelatedOrganisation();

            CommonActions.ClickLinkElement(RelatedOrganisationObjects.CancelLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrganisationsController),
                nameof(OrganisationsController.RelatedOrganisations)).Should().BeTrue();
        }

        [Fact]
        public async Task RemoveRelatedOrganisation_ClickContinue_DisplaysCorrectPage()
        {
            var organisation = await AddRelatedOrganisation();

            CommonActions.ClickLinkElement(CommonSelectors.SubmitButton);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrganisationsController),
                nameof(OrganisationsController.RelatedOrganisations)).Should().BeTrue();

            await using var context = GetEndToEndDbContext();

            var relatedOrganisations = context.RelatedOrganisations
                .Where(x => x.OrganisationId == OrganisationId
                    && x.RelatedOrganisationId == organisation.Id)
                .ToList();

            relatedOrganisations.Should().BeEmpty();
        }

        public void Dispose()
        {
            using var context = GetEndToEndDbContext();
            var relationships = context.RelatedOrganisations.Where(x => x.OrganisationId == OrganisationId).ToList();
            relationships.ForEach(x => context.RelatedOrganisations.Remove(x));
            context.SaveChanges();
        }

        private async Task<Organisation> AddRelatedOrganisation()
        {
            await using var context = GetEndToEndDbContext();
            var organisation = context.Organisations.First(x => x.Id == RelatedOrganisationId);
            context.RelatedOrganisations.Add(new RelatedOrganisation(OrganisationId, RelatedOrganisationId));
            await context.SaveChangesAsync();
            Driver.Navigate().Refresh();

            return organisation;
        }
    }
}
