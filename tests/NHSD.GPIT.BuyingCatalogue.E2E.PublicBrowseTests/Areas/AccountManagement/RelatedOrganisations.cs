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
    public class RelatedOrganisations : AccountManagerTestBase, IDisposable
    {
        private const int OrganisationId = 176;
        private const int RelatedOrganisationId = 3;

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(OrganisationId), OrganisationId.ToString() },
        };

        public RelatedOrganisations(LocalWebApplicationFactory factory)
            : base(factory, typeof(ManageAccountController), nameof(ManageAccountController.RelatedOrganisations), Parameters)
        {
        }

        [Fact]
        public void RelatedOrganisations_WithNoRelatedOrganisations_AllElementsDisplayed()
        {
            CommonActions.ElementIsDisplayed(BreadcrumbObjects.HomeBreadcrumbLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(BreadcrumbObjects.OrganisationDetailsBreadcrumbLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.Header1).Should().BeTrue();
            CommonActions.ElementExists(RelatedOrganisationObjects.RelatedOrganisationsTable).Should().BeFalse();
            CommonActions.ElementIsDisplayed(RelatedOrganisationObjects.RelatedOrganisationsErrorMessage).Should().BeTrue();
            CommonActions.ElementIsDisplayed(RelatedOrganisationObjects.ContinueLink).Should().BeTrue();
        }

        [Fact]
        public async Task RelatedOrganisations_WithRelatedOrganisations_AllElementsDisplayed()
        {
            var organisation = await AddRelatedOrganisation();

            CommonActions.ElementIsDisplayed(BreadcrumbObjects.HomeBreadcrumbLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(BreadcrumbObjects.OrganisationDetailsBreadcrumbLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.Header1).Should().BeTrue();
            CommonActions.ElementIsDisplayed(RelatedOrganisationObjects.RelatedOrganisationsTable).Should().BeTrue();
            CommonActions.ElementExists(RelatedOrganisationObjects.RelatedOrganisationsErrorMessage).Should().BeFalse();
            CommonActions.ElementIsDisplayed(RelatedOrganisationObjects.ContinueLink).Should().BeTrue();

            CommonActions.ElementTextEqualTo(RelatedOrganisationObjects.RelatedOrganisationName, organisation.Name);
            CommonActions.ElementTextEqualTo(RelatedOrganisationObjects.RelatedOrganisationsOdsCode, organisation.ExternalIdentifier);
            CommonActions.ElementIsDisplayed(RelatedOrganisationObjects.RemoveRelatedOrganisationLink).Should().BeTrue();
        }

        [Fact]
        public void RelatedOrganisations_ClickHomeBreadcrumbLink_DisplaysCorrectPage()
        {
            CommonActions.ClickLinkElement(BreadcrumbObjects.HomeBreadcrumbLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(WebApp.Controllers.HomeController),
                nameof(WebApp.Controllers.HomeController.Index)).Should().BeTrue();
        }

        [Fact]
        public void RelatedOrganisations_ClickOrganisationDetailsBreadcrumbLink_DisplaysCorrectPage()
        {
            CommonActions.ClickLinkElement(BreadcrumbObjects.OrganisationDetailsBreadcrumbLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ManageAccountController),
                nameof(ManageAccountController.Details)).Should().BeTrue();
        }

        [Fact]
        public async Task RelatedOrganisations_ClickRemoveRelatedOrganisationLink_DisplaysCorrectPage()
        {
            await AddRelatedOrganisation();

            CommonActions.ClickLinkElement(RelatedOrganisationObjects.RemoveRelatedOrganisationLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ManageAccountController),
                nameof(ManageAccountController.RemoveRelatedOrganisation)).Should().BeTrue();
        }

        [Fact]
        public async Task RelatedOrganisations_ClickContinueLink_DisplaysCorrectPage()
        {
            await AddRelatedOrganisation();

            CommonActions.ClickLinkElement(RelatedOrganisationObjects.ContinueLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ManageAccountController),
                nameof(ManageAccountController.Details)).Should().BeTrue();
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
