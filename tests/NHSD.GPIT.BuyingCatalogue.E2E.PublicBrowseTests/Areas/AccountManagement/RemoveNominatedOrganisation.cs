using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common.Organisation;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.AccountManagement.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.AccountManagement
{
    [Collection(nameof(SharedContextCollection))]
    public class RemoveNominatedOrganisation : AccountManagerTestBase, IDisposable
    {
        private const int OrganisationId = 77;
        private const int NominatedOrganisationId = 5;

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(OrganisationId), OrganisationId.ToString() },
            { nameof(NominatedOrganisationId), NominatedOrganisationId.ToString() },
        };

        public RemoveNominatedOrganisation(LocalWebApplicationFactory factory)
            : base(factory, typeof(ManageAccountController), nameof(ManageAccountController.RemoveNominatedOrganisation), Parameters)
        {
        }

        [Fact]
        public async Task RemoveNominatedOrganisation_AllElementsDisplayed()
        {
            await AddNominatedOrganisation();

            CommonActions.ElementIsDisplayed(CommonSelectors.GoBackLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.Header1).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.SubmitButton).Should().BeTrue();
            CommonActions.ElementIsDisplayed(NominatedOrganisationObjects.CancelLink).Should().BeTrue();
        }

        [Fact]
        public async Task RemoveNominatedOrganisation_ClickGoBackLink_DisplaysCorrectPage()
        {
            await AddNominatedOrganisation();

            CommonActions.ClickLinkElement(CommonSelectors.GoBackLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ManageAccountController),
                nameof(ManageAccountController.NominatedOrganisations)).Should().BeTrue();
        }

        [Fact]
        public async Task RemoveNominatedOrganisation_ClickCancelLink_DisplaysCorrectPage()
        {
            await AddNominatedOrganisation();

            CommonActions.ClickLinkElement(NominatedOrganisationObjects.CancelLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ManageAccountController),
                nameof(ManageAccountController.NominatedOrganisations)).Should().BeTrue();
        }

        [Fact]
        public async Task RemoveNominatedOrganisation_ClickContinue_DisplaysCorrectPage()
        {
            var organisation = await AddNominatedOrganisation();

            CommonActions.ClickLinkElement(CommonSelectors.SubmitButton);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ManageAccountController),
                nameof(ManageAccountController.NominatedOrganisations)).Should().BeTrue();

            await using var context = GetEndToEndDbContext();

            var nominatedOrganisations = context.RelatedOrganisations
                .Where(x => x.RelatedOrganisationId == OrganisationId
                    && x.OrganisationId == organisation.Id)
                .ToList();

            nominatedOrganisations.Should().BeEmpty();
        }

        public void Dispose()
        {
            using var context = GetEndToEndDbContext();
            var relationships = context.RelatedOrganisations.Where(x => x.RelatedOrganisationId == OrganisationId).ToList();
            relationships.ForEach(x => context.RelatedOrganisations.Remove(x));
            context.SaveChanges();
        }

        private async Task<Organisation> AddNominatedOrganisation()
        {
            await using var context = GetEndToEndDbContext();
            var organisation = context.Organisations.First(x => x.Id == NominatedOrganisationId);
            context.RelatedOrganisations.Add(new RelatedOrganisation(NominatedOrganisationId, OrganisationId));
            await context.SaveChangesAsync();
            Driver.Navigate().Refresh();

            return organisation;
        }
    }
}
