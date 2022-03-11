using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Admin;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.Organisations
{
    public sealed class Details : AuthorityTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private const int OrganisationId = 2;

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(OrganisationId), OrganisationId.ToString() },
        };

        public Details(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(OrganisationsController),
                  nameof(OrganisationsController.Details),
                  Parameters)
        {
        }

        [Fact]
        public async Task Details_OrganisationDetailsDisplayed()
        {
            await using var context = GetEndToEndDbContext();
            var dbAddress = (await context.Organisations.SingleAsync(s => s.Id == OrganisationId)).Address;

            var pageAddress = AdminPages.Organisation.GetAddress().ToList();

            foreach (var prop in dbAddress.GetType().GetProperties())
            {
                if (prop.GetValue(dbAddress) is null)
                    continue;

                var value = prop.GetValue(dbAddress)?.ToString();
                pageAddress.First().Should().Contain(value);
            }
        }

        [Fact]
        public async Task Details_OdsCodeDisplayed()
        {
            await using var context = GetEndToEndDbContext();
            var dbOdsCode = (await context.Organisations.SingleAsync(s => s.Id == OrganisationId)).ExternalIdentifier;

            var pageOdsCode = AdminPages.Organisation.GetOdsCode();

            pageOdsCode.Should().Be(dbOdsCode);
        }

        [Fact]
        public void Details_AllLinksDisplayed()
        {
            CommonActions.ElementIsDisplayed(OrganisationObjects.HomeBreadcrumbLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(OrganisationObjects.ManageBuyerOrganisationsBreadcrumbLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(OrganisationObjects.UserAccountsLink).Should().BeTrue();

            AdminPages.Organisation.AddRelatedOrganisationButtonDisplayed().Should().BeTrue();
            AdminPages.Organisation.RelatedOrganisationTableDisplayed().Should().BeTrue();
        }

        [Fact]
        public void Details_ClickHomeBreadcrumbLink_ExpectedResult()
        {
            CommonActions.ClickLinkElement(OrganisationObjects.HomeBreadcrumbLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(HomeController),
                nameof(HomeController.Index)).Should().BeTrue();
        }

        [Fact]
        public void Details_ClickManageBuyerOrganisationsBreadcrumbLink_ExpectedResult()
        {
            CommonActions.ClickLinkElement(OrganisationObjects.ManageBuyerOrganisationsBreadcrumbLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrganisationsController),
                nameof(OrganisationsController.Index)).Should().BeTrue();
        }

        [Fact]
        public void Details_ClickUserAccountsLink_ExpectedResult()
        {
            CommonActions.ClickLinkElement(OrganisationObjects.UserAccountsLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrganisationsController),
                nameof(OrganisationsController.Users)).Should().BeTrue();
        }

        [Fact]
        public async Task Details_AddRelatedOrganisation()
        {
            AdminPages.Organisation.ClickAddRelatedOrgButton();

            var relatedOrgId = AdminPages.AddRelatedOrganisation.SelectOrganisation(OrganisationId);

            AdminPages.AddRelatedOrganisation.ClickAddRelatedOrgButton();

            var organisation = AdminPages.Organisation.GetRelatedOrganisation(relatedOrgId);

            await using var context = GetEndToEndDbContext();
            var relatedOrgIds = await context.RelatedOrganisations.Where(s => s.OrganisationId == OrganisationId).ToListAsync();
            relatedOrgIds.Select(s => s.RelatedOrganisationId).Should().Contain(relatedOrgId);

            var relatedOrganisation = await context.Organisations.SingleAsync(s => s.Id == relatedOrgId);

            organisation.OrganisationName.Should().BeEquivalentTo(relatedOrganisation.Name);
            organisation.OdsCode.Should().BeEquivalentTo(relatedOrganisation.ExternalIdentifier);
        }

        [Fact]
        public async Task Details_RemoveRelatedOrganisation()
        {
            var relatedOrgId = await AddRelatedOrganisation();

            AdminPages.Organisation.RemoveRelatedOrganisation(relatedOrgId);

            await using var context = GetEndToEndDbContext();
            var relationships = await context.RelatedOrganisations.ToListAsync();
            var selectedRelationships = relationships.Where(o => o.OrganisationId == OrganisationId);

            selectedRelationships.Select(s => s.RelatedOrganisationId).Should().NotContain(relatedOrgId);
        }

        private async Task<int> AddRelatedOrganisation()
        {
            await using var context = GetEndToEndDbContext();
            var organisations = await context.Organisations.Where(o => o.Id != OrganisationId).ToListAsync();

            var relatedOrganisation = new RelatedOrganisation
            {
                OrganisationId = OrganisationId,
                RelatedOrganisationId = organisations[new Random().Next(organisations.Count)].Id,
            };

            context.RelatedOrganisations.Add(relatedOrganisation);
            await context.SaveChangesAsync();

            Driver.Navigate().Refresh();

            return relatedOrganisation.RelatedOrganisationId;
        }
    }
}
