using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Marketing;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.AddNewSolution
{
    public sealed class HostingType : TestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public HostingType(LocalWebApplicationFactory factory)
           : base(factory, "/admin/catalogue-solutions/manage/99999-002/hosting-type")
        {
            AuthorityLogin();
        }

        [Fact]
        public async Task HostingType_TitleDisplayedCorrectly()
        {
            await using var context = GetEndToEndDbContext();
            var solutionName = (await context.CatalogueItems.SingleAsync(s => s.CatalogueItemId == new CatalogueItemId(99999, "002"))).Name;

            CommonActions.PageTitle()
                .Should()
                .BeEquivalentTo($"Hosting Type - {solutionName}".FormatForComparison());
        }

        [Fact]

        public async Task HostingType_PublicCloud_CompleteAllFields()
        {
            AdminPages.CommonActions.ClickAddHostingTypeLink();
            CommonActions.ClickRadioButtonWithText("Public cloud");

            CommonActions.ClickSave();

            var summary = TextGenerators.TextInputAddText(HostingTypesObjects.HostingType_Summary, 500);
            var link = TextGenerators.UrlInputAddText(HostingTypesObjects.HostingType_Link, 1000);
            var expected = new ServiceContracts.Solutions.PublicCloud
            {
                Link = link,
                Summary = summary,
            };

            MarketingPages.HostingTypeActions.ToggleHSCNCheckbox();

            CommonActions.ClickSave();

            await using var context = GetEndToEndDbContext();
            var hosting = (await context.Solutions.SingleAsync(s => s.Id == new CatalogueItemId(99999, "002"))).Hosting;

            var actual = JsonConvert.DeserializeObject<ServiceContracts.Solutions.Hosting>(hosting);
            actual.PublicCloud.Should().BeEquivalentTo(expected, opt => opt.Excluding(p => p.RequiresHscn));
        }

        [Fact]
        public async Task HostingType_PrivateCloud_CompleteAllFields()
        {
            AdminPages.CommonActions.ClickAddHostingTypeLink();
            CommonActions.ClickRadioButtonWithText("Private cloud");

            CommonActions.ClickSave();

            var summary = TextGenerators.TextInputAddText(HostingTypesObjects.HostingType_Summary, 500);
            var link = TextGenerators.UrlInputAddText(HostingTypesObjects.HostingType_Link, 1000);
            var hostingModel = TextGenerators.TextInputAddText(HostingTypesObjects.HostingType_HostingModel, 1000);
            var expected = new ServiceContracts.Solutions.PrivateCloud
            {
                HostingModel = hostingModel,
                Link = link,
                Summary = summary,
            };

            MarketingPages.HostingTypeActions.ToggleHSCNCheckbox();
            CommonActions.ClickSave();

            await using var context = GetEndToEndDbContext();
            var hosting = (await context.Solutions.SingleAsync(s => s.Id == new CatalogueItemId(99999, "002"))).Hosting;

            var actual = JsonConvert.DeserializeObject<ServiceContracts.Solutions.Hosting>(hosting);
            actual.PrivateCloud.Should().BeEquivalentTo(expected, opt => opt.Excluding(p => p.RequiresHscn));
        }

        [Fact]
        public async Task HostingType_Hybrid_CompleteAllFields()
        {
            AdminPages.CommonActions.ClickAddHostingTypeLink();
            CommonActions.ClickRadioButtonWithText("Hybrid");

            CommonActions.ClickSave();

            var summary = TextGenerators.TextInputAddText(HostingTypesObjects.HostingType_Summary, 500);
            var link = TextGenerators.UrlInputAddText(HostingTypesObjects.HostingType_Link, 1000);
            var hostingModel = TextGenerators.TextInputAddText(HostingTypesObjects.HostingType_HostingModel, 1000);
            var expected = new ServiceContracts.Solutions.HybridHostingType
            {
                HostingModel = hostingModel,
                Link = link,
                Summary = summary,
            };

            MarketingPages.HostingTypeActions.ToggleHSCNCheckbox();

            CommonActions.ClickSave();

            await using var context = GetEndToEndDbContext();
            var hosting = (await context.Solutions.SingleAsync(s => s.Id == new CatalogueItemId(99999, "002"))).Hosting;

            var actual = JsonConvert.DeserializeObject<ServiceContracts.Solutions.Hosting>(hosting);
            actual.HybridHostingType.Should().BeEquivalentTo(expected, opt => opt.Excluding(h => h.RequiresHscn));
        }

        [Fact]
        public async Task HostingType_OnPremise_CompleteAllFields()
        {
            AdminPages.CommonActions.ClickAddHostingTypeLink();
            CommonActions.ClickRadioButtonWithText("On premise");

            CommonActions.ClickSave();

            var summary = TextGenerators.TextInputAddText(HostingTypesObjects.HostingType_Summary, 500);
            var link = TextGenerators.UrlInputAddText(HostingTypesObjects.HostingType_Link, 1000);
            var hostingModel = TextGenerators.TextInputAddText(HostingTypesObjects.HostingType_HostingModel, 1000);
            var expected = new ServiceContracts.Solutions.OnPremise
            {
                Summary = summary,
                Link = link,
                HostingModel = hostingModel,
            };

            MarketingPages.HostingTypeActions.ToggleHSCNCheckbox();

            CommonActions.ClickSave();

            await using var context = GetEndToEndDbContext();
            var hosting = (await context.Solutions.SingleAsync(s => s.Id == new CatalogueItemId(99999, "002"))).Hosting;

            var actual = JsonConvert.DeserializeObject<ServiceContracts.Solutions.Hosting>(hosting);
            actual.OnPremise.Should().BeEquivalentTo(expected, opt => opt.Excluding(p => p.RequiresHscn));
        }
    }
}
