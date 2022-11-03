﻿using System;
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
    public class NominatedOrganisations : AccountManagerTestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        private const int OrganisationId = 1;
        private const int RelatedOrganisationId = 5;

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(OrganisationId), OrganisationId.ToString() },
        };

        public NominatedOrganisations(LocalWebApplicationFactory factory)
            : base(factory, typeof(ManageAccountController), nameof(ManageAccountController.NominatedOrganisations), Parameters)
        {
        }

        [Fact]
        public void NominatedOrganisations_WithNoNominatedOrganisations_AllElementsDisplayed()
        {
            CommonActions.ElementIsDisplayed(BreadcrumbObjects.HomeBreadcrumbLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(BreadcrumbObjects.OrganisationDetailsBreadcrumbLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.Header1).Should().BeTrue();
            CommonActions.ElementExists(NominatedOrganisationObjects.NominatedOrganisationsTable).Should().BeFalse();
            CommonActions.ElementIsDisplayed(NominatedOrganisationObjects.NominatedOrganisationsErrorMessage).Should().BeTrue();
            CommonActions.ElementIsDisplayed(NominatedOrganisationObjects.ContinueLink).Should().BeTrue();
        }

        [Fact]
        public async Task NominatedOrganisations_WithNominatedOrganisations_AllElementsDisplayed()
        {
            var organisation = await AddNominatedOrganisation();

            CommonActions.ElementIsDisplayed(BreadcrumbObjects.HomeBreadcrumbLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(BreadcrumbObjects.OrganisationDetailsBreadcrumbLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.Header1).Should().BeTrue();
            CommonActions.ElementIsDisplayed(NominatedOrganisationObjects.NominatedOrganisationsTable).Should().BeTrue();
            CommonActions.ElementExists(NominatedOrganisationObjects.NominatedOrganisationsErrorMessage).Should().BeFalse();
            CommonActions.ElementIsDisplayed(NominatedOrganisationObjects.ContinueLink).Should().BeTrue();

            CommonActions.ElementTextEqualTo(NominatedOrganisationObjects.NominatedOrganisationName, organisation.Name);
            CommonActions.ElementTextEqualTo(NominatedOrganisationObjects.NominatedOrganisationsOdsCode, organisation.ExternalIdentifier);
            CommonActions.ElementIsDisplayed(NominatedOrganisationObjects.RemoveNominatedOrganisationLink).Should().BeTrue();
        }

        [Fact]
        public void NominatedOrganisations_ClickHomeBreadcrumbLink_DisplaysCorrectPage()
        {
            CommonActions.ClickLinkElement(BreadcrumbObjects.HomeBreadcrumbLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(WebApp.Controllers.HomeController),
                nameof(WebApp.Controllers.HomeController.Index)).Should().BeTrue();
        }

        [Fact]
        public void NominatedOrganisations_ClickOrganisationDetailsBreadcrumbLink_DisplaysCorrectPage()
        {
            CommonActions.ClickLinkElement(BreadcrumbObjects.OrganisationDetailsBreadcrumbLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ManageAccountController),
                nameof(ManageAccountController.Details)).Should().BeTrue();
        }

        [Fact]
        public async Task NominatedOrganisations_ClickRemoveRelatedOrganisationLink_DisplaysCorrectPage()
        {
            await AddNominatedOrganisation();

            CommonActions.ClickLinkElement(NominatedOrganisationObjects.RemoveNominatedOrganisationLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ManageAccountController),
                nameof(ManageAccountController.RemoveNominatedOrganisation)).Should().BeTrue();
        }

        [Fact]
        public async Task NominatedOrganisations_ClickContinueLink_DisplaysCorrectPage()
        {
            await AddNominatedOrganisation();

            CommonActions.ClickLinkElement(NominatedOrganisationObjects.ContinueLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ManageAccountController),
                nameof(ManageAccountController.Details)).Should().BeTrue();
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
            var organisation = context.Organisations.Single(x => x.Id == RelatedOrganisationId);
            context.RelatedOrganisations.Add(new RelatedOrganisation(RelatedOrganisationId, OrganisationId));
            await context.SaveChangesAsync();
            Driver.Navigate().Refresh();

            return organisation;
        }
    }
}
