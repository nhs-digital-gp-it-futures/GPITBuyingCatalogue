using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin
{
    public sealed class AdminDashboard : TestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public AdminDashboard(LocalWebApplicationFactory factory) : base(factory, "admin/buyer-organisations")
        {
            Login();
        }

        [Fact]
        public void AdminDashboard_AddOrgButtonDisplayed()
        {
            AdminPages.Dashboard.AddOrgButtonDisplayed().Should().BeTrue();
        }

        [Fact]
        public async Task AdminDashboard_AllOrgsDisplayed()
        {
            await using var context = GetEndToEndDbContext();
            var organisationNames = await context.Organisations.Select(s => s.Name).ToListAsync();
            var organisationIds = await context.Organisations.Select(s => s.OrganisationId).ToListAsync();
            var otherProgramList = organisationIds.Select(x => x.ToString()).ToList();
            var organisationOdsCodes = await context.Organisations.Select(s => s.OdsCode).ToListAsync();

            var orgNames = AdminPages.Dashboard.GetOrgNamesOnPage();
            var orgCodes = AdminPages.Dashboard.GetOrgOdsCodesOnPage();
            var orgLinkIds = AdminPages.Dashboard.GetOrgLinkIdsOnPage();

            orgNames.Should().HaveCount(organisationNames.Count());
            orgLinkIds.Should().BeEquivalentTo(otherProgramList);
            orgCodes.Should().BeEquivalentTo(organisationOdsCodes);
            orgNames.Should().BeEquivalentTo(organisationNames);
        }
    }
}
