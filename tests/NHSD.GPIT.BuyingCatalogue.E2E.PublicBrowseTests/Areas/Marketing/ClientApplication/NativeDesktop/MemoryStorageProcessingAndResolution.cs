using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Marketing.ClientApplication.NativeDesktop
{
    public sealed class MemoryStorageProcessingAndResolution : TestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public MemoryStorageProcessingAndResolution(LocalWebApplicationFactory factory) : base(factory, "marketing/supplier/solution/99999-99/section/native-desktop/memory-and-storage")
        {
            ClearClientApplication("99999-99");
            driver.Navigate().Refresh();
        }

        [Fact]
        public async Task MemoryStorageProcessingAndResolution_CompleteAllFields()
        {
            MarketingPages.ClientApplicationTypeActions.SelectMemoryDropdown(1);

            var storageSpace = MarketingPages.ClientApplicationTypeActions.EnterStorageSpaceText(300);

            var processingPower = MarketingPages.ClientApplicationTypeActions.EnterProcessingPowerText(300);

            MarketingPages.ClientApplicationTypeActions.SelectResolutionDropdown(1);

            MarketingPages.CommonActions.ClickSave();

            using var context = GetBCContext();

            var clientApplication = (await context.Solutions.SingleAsync(s => s.Id == "99999-99")).ClientApplication;
            clientApplication.Should().ContainEquivalentOf(@"MinimumMemoryRequirement"":""256MB""");
            clientApplication.Should().ContainEquivalentOf($"StorageRequirementsDescription\":\"{storageSpace}\"");
            clientApplication.Should().ContainEquivalentOf($"MinimumCpu\":\"{processingPower}\"");
            clientApplication.Should().ContainEquivalentOf(@"RecommendedResolution"":""16:9 - 640 x 360");
        }

        [Fact]
        public void MemoryStorageProcessingAndResolution_SectionComplete()
        {
            MarketingPages.ClientApplicationTypeActions.SelectMemoryDropdown(1);

            MarketingPages.ClientApplicationTypeActions.EnterStorageSpaceText(300);

            MarketingPages.ClientApplicationTypeActions.EnterProcessingPowerText(300);

            MarketingPages.ClientApplicationTypeActions.SelectResolutionDropdown(1);

            MarketingPages.CommonActions.ClickSave();

            MarketingPages.DashboardActions.SectionMarkedComplete("Memory, storage, processing and resolution").Should().BeTrue();
        }

        [Fact]
        public void MemoryStorageProcessingAndResolution_SectionIncomplete()
        {
            MarketingPages.CommonActions.ClickGoBackLink();

            MarketingPages.DashboardActions.SectionMarkedComplete("Memory, storage, processing and resolution").Should().BeFalse();
        }
    }
}
