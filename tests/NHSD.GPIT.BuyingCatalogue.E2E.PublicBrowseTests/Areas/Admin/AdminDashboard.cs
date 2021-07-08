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
            : base(factory, "admin/buyer-organisations")
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
            var organisations = await context.Organisations.Select(o => new
                {
                    o.Name,
                    o.OrganisationId,
                    o.OdsCode,
                })
                .ToListAsync();

            var expectedOrganisationNames = organisations.Select(o => o.Name).ToList();
            var expectedOrganisationIds = organisations.Select(o => o.OrganisationId.ToString()).ToList();
            var expectedOrganisationOdsCodes = organisations.Select(o => o.OdsCode).ToList();

            var actualOrganisationNames = AdminPages.Dashboard.GetOrganisationNamesOnPage();
            var actualOrganisationCodes = AdminPages.Dashboard.GetOrganisationOdsCodesOnPage();
            var actualOrganisationIdLinks = AdminPages.Dashboard.GetOrganisationLinkIdsOnPage();

            actualOrganisationNames.Should().HaveCount(expectedOrganisationNames.Count);
            actualOrganisationCodes.Should().BeEquivalentTo(expectedOrganisationOdsCodes);
            actualOrganisationNames.Should().BeEquivalentTo(expectedOrganisationNames);
            actualOrganisationIdLinks.Should().BeEquivalentTo(expectedOrganisationIds);
        }
    }
}
