using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Marketing;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Marketing.Hosting
{
    public sealed class PublicCloud : TestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        public PublicCloud(LocalWebApplicationFactory factory) : base(factory, "marketing/supplier/solution/99999-99/section/hosting-type-public-cloud")
        {
        }

        [Fact]
        public async Task PublicCloud_CompleteAllFields()
        {
            var summary = TextGenerators.TextInputAddText(HostingTypesObjects.PublicCloud_Summary, 500);
            var link = TextGenerators.UrlInputAddText(HostingTypesObjects.PublicCloud_Link, 1000);
            var expected = new ServiceContracts.Solutions.PublicCloud
            {
                Link = link,
                Summary = summary,
            };
            
            MarketingPages.HostingTypeActions.ToggleHSCNCheckbox();

            CommonActions.ClickSave();

            using var context = GetBCContext();
            var hosting = (await context.Solutions.SingleAsync(s => s.Id == "99999-99")).Hosting;

            var actual = JsonConvert.DeserializeObject<ServiceContracts.Solutions.Hosting>(hosting);
            actual.PublicCloud.Should().BeEquivalentTo(expected, opt => opt.Excluding(p => p.RequiresHscn));
        }

        [Fact]
        public void PublicCloud_SectionComplete()
        {
            TextGenerators.TextInputAddText(HostingTypesObjects.PublicCloud_Summary, 500);
            TextGenerators.UrlInputAddText(HostingTypesObjects.PublicCloud_Link, 1000);
            MarketingPages.HostingTypeActions.ToggleHSCNCheckbox();

            CommonActions.ClickSave();

            MarketingPages.DashboardActions.SectionMarkedComplete("Public cloud").Should().BeTrue();
        }

        [Fact]
        public void PublicCloud_SectionIncomplete()
        {
            CommonActions.ClickGoBackLink();

            MarketingPages.DashboardActions.SectionMarkedComplete("Public cloud").Should().BeFalse();
        }

        public void Dispose()
        {
            ClearHostingTypes("99999-99");
        }
    }
}
