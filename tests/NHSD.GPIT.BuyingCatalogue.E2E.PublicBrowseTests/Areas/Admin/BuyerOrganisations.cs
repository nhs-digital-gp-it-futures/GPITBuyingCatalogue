using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin
{
    public sealed class BuyerOrganisations : AuthorityTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public BuyerOrganisations(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(HomeController),
                  nameof(HomeController.BuyerOrganisations),
                  null)
        {
        }

        [Fact]
        public void BuyerOrganisations_ManageBuyerOrgsAndUsers_AddOrgLinkDisplayed()
        {
            AdminPages.Dashboard.AddOrgLinkDisplayed().Should().BeTrue();
        }

        [Fact]
        public void BuyerOrganisations_ManageBuyerOrgsAndUsers_OrganisationTableListDisplayed()
        {
            AdminPages.Organisation.BuyerOrgsTableDisplayed().Should().BeTrue();
        }

        [Fact]
        public async Task BuyerOrganisations_ManageBuyerOrgsAndUsers_AllOrgsDisplayed()
        {
            var actualOrganisationNames = AdminPages.Dashboard.GetOrganisationNamesOnPage();
            var actualOrganisationCodes = AdminPages.Dashboard.GetOrganisationOdsCodesOnPage();
            var actualOrganisationIdLinks = AdminPages.Dashboard.GetOrganisationLinkIdsOnPage();

            await using var context = GetEndToEndDbContext();
            var organisations = await context.Organisations.Select(o => new
            {
                o.Name,
                o.OdsCode,
                o.OrganisationId,
            }).ToListAsync();

            actualOrganisationNames
                .Should().HaveCount(organisations.Select(o => o.Name).Count())
                .And.BeEquivalentTo(organisations.Select(o => o.Name));
            actualOrganisationCodes.Should().BeEquivalentTo(organisations.Select(o => o.OdsCode));
            actualOrganisationIdLinks.Select(o => Guid.Parse(o)).Should().BeEquivalentTo(organisations.Select(o => o.OrganisationId));
        }
    }
}
