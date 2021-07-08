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
            : base(factory, "admin/organisations")
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
            var organisations = await context.Organisations.Select(s => s.Name).ToListAsync();

            var orgsDisplayed = AdminPages.Dashboard.GetOrgsOnPage();

            orgsDisplayed.Should().HaveCount(organisations.Count);

            orgsDisplayed.Should().BeEquivalentTo(organisations);
        }
    }
}
