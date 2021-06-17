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
        public AdminDashboard(LocalWebApplicationFactory factory) : base(factory, "admin/organisations")
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
            using var context = GetBCContext();
            var organisations = await context.Organisations.Select(s => s.Name).ToListAsync();

            var orgsDisplayed = AdminPages.Dashboard.GetOrgsOnPage();

            orgsDisplayed.Should().HaveCount(organisations.Count());

            orgsDisplayed.Should().BeEquivalentTo(organisations);
        }
    }
}
