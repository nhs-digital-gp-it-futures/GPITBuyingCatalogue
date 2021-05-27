using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Marketing;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Marketing.ClientApplication.NativeMobile
{
    // TODO: fix when page updated with new layout
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1000:Test classes must be public", Justification = "Disabled")]
    internal sealed class MemoryAndStorage : TestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        public MemoryAndStorage(LocalWebApplicationFactory factory) : base(factory, "marketing/supplier/solution/99999-99/section/native-mobile/memory-and-storage")
        {
        }

        [Fact]
        public async Task MemoryAndStorage_CompleteAllFields()
        {
            CommonActions.SelectDropdownItem(CommonSelectors.MemorySelect, 1);

            var description = TextGenerators.TextInputAddText(CommonSelectors.Description, 200);

            CommonActions.ClickSave();

            using var context = GetBCContext();

            var clientApplication = (await context.Solutions.SingleAsync(s => s.Id == "99999-99")).ClientApplication;

            clientApplication.Should().ContainEquivalentOf("MinimumMemoryRequirement\":\"256MB\"");
            clientApplication.Should().ContainEquivalentOf($"Description\":\"{description}\"");
        }

        [Fact]
        public void MemoryAndStorage_SectionComplete()
        {
            CommonActions.SelectDropdownItem(CommonSelectors.MemorySelect, 1);

            TextGenerators.TextInputAddText(CommonSelectors.Description, 200);

            CommonActions.ClickSave();

            MarketingPages.DashboardActions.SectionMarkedComplete("Memory and storage").Should().BeTrue();
        }

        [Fact]
        public void MemoryAndStorage_SectionIncomplete()
        {
            CommonActions.ClickGoBackLink();

            MarketingPages.DashboardActions.SectionMarkedComplete("Memory and storage").Should().BeFalse();
        }

        public void Dispose()
        {
            ClearClientApplication("99999-99");
        }
    }
}
