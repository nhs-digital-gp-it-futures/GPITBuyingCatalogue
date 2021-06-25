using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Marketing;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Marketing.Hosting
{
    public sealed class PrivateCloud : TestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        public PrivateCloud(LocalWebApplicationFactory factory) : base(factory, "marketing/supplier/solution/99999-99/section/hosting-type-private-cloud")
        {
        }

        [Fact]
        public async Task PrivateCloud_CompleteAllFields()
        {
            var summary = TextGenerators.TextInputAddText(HostingTypesObjects.PrivateCloud_Summary, 500);
            var link = TextGenerators.UrlInputAddText(HostingTypesObjects.PrivateCloud_Link, 1000);
            var hostingModel = TextGenerators.TextInputAddText(HostingTypesObjects.PrivateCloud_HostingModel, 1000);
            var expected = new ServiceContracts.Solutions.PrivateCloud
            {
                HostingModel = hostingModel,
                Link = link,
                Summary = summary,
            };

            MarketingPages.HostingTypeActions.ToggleHSCNCheckbox();
            CommonActions.ClickSave();

            await using var context = GetEndToEndDbContext();
            var hosting = (await context.Solutions.SingleAsync(s => s.Id == new CatalogueItemId(99999, "99"))).Hosting;

            var actual = JsonConvert.DeserializeObject<ServiceContracts.Solutions.Hosting>(hosting);
            actual.PrivateCloud.Should().BeEquivalentTo(expected, opt => opt.Excluding(p => p.RequiresHscn));
        }

        [Fact]
        public void PrivateCloud_SectionComplete()
        {
            TextGenerators.TextInputAddText(HostingTypesObjects.PrivateCloud_Summary, 500);
            TextGenerators.UrlInputAddText(HostingTypesObjects.PrivateCloud_Link, 1000);
            TextGenerators.TextInputAddText(HostingTypesObjects.PrivateCloud_HostingModel, 1000);
            MarketingPages.HostingTypeActions.ToggleHSCNCheckbox();

            CommonActions.ClickSave();

            MarketingPages.DashboardActions.SectionMarkedComplete("Private cloud").Should().BeTrue();
        }

        [Fact]
        public void PrivateCloud_SectionIncomplete()
        {
            CommonActions.ClickGoBackLink();

            MarketingPages.DashboardActions.SectionMarkedComplete("Private cloud").Should().BeFalse();
        }

        public void Dispose()
        {
            ClearHostingTypes(new CatalogueItemId(99999, "99"));
        }
    }
}
