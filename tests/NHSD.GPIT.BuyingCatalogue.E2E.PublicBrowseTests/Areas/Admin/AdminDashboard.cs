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
        public AdminDashboard(LocalWebApplicationFactory factory)
            : base(factory, "admin")
        {
            Login();
        }

        [Fact]
        public void AdminDashboard_ManageBuyerOrgsAndUsers_AddOrgLinkDisplayed()
        {
            AdminPages.Dashboard.ClickBuyerOrgLink();
            AdminPages.Dashboard.AddOrgButtonDisplayed().Should().BeTrue();
        }

        [Fact]
        public void AdminDashboard_ManageBuyerOrgsAndUsers_OrganisationTableListDisplayed()
        {
            AdminPages.Dashboard.ClickBuyerOrgLink();
            AdminPages.Organisation.BuyerOrgsTableDisplayed().Should().BeTrue();
        }

        [Fact]
        public async Task AdminDashboard_ManageBuyerOrgsAndUsers_AllOrgsDisplayed()
        {
            AdminPages.Dashboard.ClickBuyerOrgLink();
            var actualOrganisationNames = AdminPages.Dashboard.GetOrganisationNamesOnPage();
            var actualOrganisationCodes = AdminPages.Dashboard.GetOrganisationOdsCodesOnPage();
            var actualOrganisationIdLinks = AdminPages.Dashboard.GetOrganisationLinkIdsOnPage();

            await using var context = GetEndToEndDbContext();
            var organisations = await context.Organisations.Select(o => new
            {
                o.Name,
                o.OdsCode,
                o.OrganisationId,
            })
                .ToListAsync();

            var expectedOrganisationNames = organisations.Select(o => o.Name).ToList();
            var expectedOrganisationOdsCodes = organisations.Select(o => o.OdsCode).ToList();
            var expectedOrganisationIds = organisations.Select(o => o.OrganisationId.ToString()).ToList();

            actualOrganisationNames.Should().HaveCount(expectedOrganisationNames.Count);
            actualOrganisationCodes.Should().BeEquivalentTo(expectedOrganisationOdsCodes);
            actualOrganisationNames.Should().BeEquivalentTo(expectedOrganisationNames);
            actualOrganisationIdLinks.Should().BeEquivalentTo(expectedOrganisationIds);
        }
    }
}
