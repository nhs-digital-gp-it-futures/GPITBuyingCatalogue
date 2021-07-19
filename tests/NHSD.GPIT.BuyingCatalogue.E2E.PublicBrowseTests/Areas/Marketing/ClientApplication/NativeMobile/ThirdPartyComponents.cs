using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Marketing;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Marketing.ClientApplication.NativeMobile
{
    public sealed class ThirdPartyComponents : TestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        public ThirdPartyComponents(LocalWebApplicationFactory factory)
            : base(factory, "marketing/supplier/solution/99999-99/section/native-mobile/third-party")
        {
            AuthorityLogin();
        }

        [Fact]
        public async Task ThirdPartyComponents_CompleteAllFields()
        {
            var thirdPartyComponent = TextGenerators.TextInputAddText(CommonSelectors.ThirdPartyComponentTextArea, 500);

            var deviceCapability = TextGenerators.TextInputAddText(CommonSelectors.DeviceCapabilityTextArea, 500);

            CommonActions.ClickSave();

            await using var context = GetEndToEndDbContext();

            var clientApplication = (await context.Solutions.SingleAsync(s => s.Id == new CatalogueItemId(99999, "99"))).ClientApplication;
            clientApplication.Should().ContainEquivalentOf($"ThirdPartyComponents\":\"{thirdPartyComponent}\"");
            clientApplication.Should().ContainEquivalentOf($"DeviceCapabilities\":\"{deviceCapability}\"");
        }

        [Fact]
        public void ThirdPartyComponents_SectionComplete()
        {
            TextGenerators.TextInputAddText(CommonSelectors.ThirdPartyComponentTextArea, 500);

            TextGenerators.TextInputAddText(CommonSelectors.DeviceCapabilityTextArea, 500);

            CommonActions.ClickSave();

            MarketingPages.DashboardActions.SectionMarkedComplete("Third-party components and device capabilities").Should().BeTrue();
        }

        [Fact]
        public void ThirdPartyComponents_SectionIncomplete()
        {
            CommonActions.ClickGoBackLink();

            MarketingPages.DashboardActions.SectionMarkedComplete("Third-party components and device capabilities").Should().BeFalse();
        }

        public void Dispose()
        {
            ClearClientApplication(new CatalogueItemId(99999, "99"));
        }
    }
}
