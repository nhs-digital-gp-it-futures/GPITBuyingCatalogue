using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.PublicBrowse.Solutions
{
    public sealed class CapabilitySelector : TestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public CapabilitySelector(LocalWebApplicationFactory factory)
            : base(factory, "solutions/futures/capabilitiesselector")
        {
        }

        [Fact]
        public async Task CapabilitySelector_AllCapabilitiesDisplayed()
        {
            await using var context = GetEndToEndDbContext();
            var capabilities = await context.FrameworkCapabilities.Include(s => s.Capability).Where(s => s.FrameworkId == "NHSDGP001").ToListAsync();

            var capabilityCheckboxes = PublicBrowsePages.CapabilitySelectorActions.GetAllCheckboxLabels();

            capabilities.Select(c => c.Capability.Name).Should().BeEquivalentTo(capabilityCheckboxes);
        }

        [Fact]
        public void CapabilitySelector_GoBack()
        {
            PublicBrowsePages.CommonActions.ClickGoBackLink();

            PublicBrowsePages.CommonActions.PageTitle().Should().ContainEquivalentOf("GP IT Futures framework");
        }

        [Fact]
        public void CapabilitySelector_NoCheckboxSelected()
        {
            PublicBrowsePages.CommonActions.ClickContinueButton();

            PublicBrowsePages.SolutionsActions.GetSolutionsInList().Should().HaveCount(2);
        }

        [Fact]
        public void CapabilitySelector_SingleCheckboxSelected()
        {
            PublicBrowsePages.CapabilitySelectorActions.ClickFirstCheckbox();

            PublicBrowsePages.CommonActions.ClickContinueButton();

            PublicBrowsePages.SolutionsActions.GetSolutionsInList().Should().HaveCount(1);
        }
    }
}
