using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EnumsNET;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Marketing;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.AddNewSolution.HostingTypes
{
    [Collection(nameof(AdminCollection))]
    public sealed class HostingType : AuthorityTestBase, IDisposable
    {
        private static readonly CatalogueItemId SolutionId = new(99999, "002");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
        };

        public HostingType(LocalWebApplicationFactory factory)
           : base(
                 factory,
                 typeof(HostingTypesController),
                 nameof(HostingTypesController.HostingType),
                 Parameters)
        {
        }

        [Fact]
        public async Task HostingType_TitleDisplayedCorrectly()
        {
            await using var context = GetEndToEndDbContext();
            var solutionName = (await context.CatalogueItems.FirstAsync(s => s.Id == new CatalogueItemId(99999, "002"))).Name;

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
            var expected = new PublicCloud
            {
                Link = link,
                Summary = summary,
            };

            MarketingPages.HostingTypeActions.ToggleHSCNCheckbox();

            CommonActions.ClickSave();

            await using var context = GetEndToEndDbContext();
            var actual = (await context.Solutions.FirstAsync(s => s.CatalogueItemId == SolutionId)).Hosting;

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
            var expected = new PrivateCloud
            {
                HostingModel = hostingModel,
                Link = link,
                Summary = summary,
            };

            MarketingPages.HostingTypeActions.ToggleHSCNCheckbox();
            CommonActions.ClickSave();

            await using var context = GetEndToEndDbContext();
            var actual = (await context.Solutions.FirstAsync(s => s.CatalogueItemId == SolutionId)).Hosting;

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
            var expected = new HybridHostingType
            {
                HostingModel = hostingModel,
                Link = link,
                Summary = summary,
            };

            MarketingPages.HostingTypeActions.ToggleHSCNCheckbox();

            CommonActions.ClickSave();

            await using var context = GetEndToEndDbContext();
            var actual = (await context.Solutions.FirstAsync(s => s.CatalogueItemId == SolutionId)).Hosting;

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
            var expected = new OnPremise
            {
                Summary = summary,
                Link = link,
                HostingModel = hostingModel,
            };

            MarketingPages.HostingTypeActions.ToggleHSCNCheckbox();

            CommonActions.ClickSave();

            await using var context = GetEndToEndDbContext();
            var actual = (await context.Solutions.FirstAsync(s => s.CatalogueItemId == SolutionId)).Hosting;

            actual.OnPremise.Should().BeEquivalentTo(expected, opt => opt.Excluding(p => p.RequiresHscn));
        }

        [Theory]
        [InlineData(ServiceContracts.Solutions.HostingType.Hybrid)]
        [InlineData(ServiceContracts.Solutions.HostingType.OnPremise)]
        [InlineData(ServiceContracts.Solutions.HostingType.PrivateCloud)]
        [InlineData(ServiceContracts.Solutions.HostingType.PublicCloud)]
        public void HostingType_Add_DoesNotContainDeleteLink(ServiceContracts.Solutions.HostingType hostingType)
        {
            AdminPages.CommonActions.ClickAddHostingTypeLink();
            CommonActions.ClickRadioButtonWithText(hostingType.AsString(EnumFormat.DisplayName));

            CommonActions.ClickSave();

            CommonActions
                .ElementIsDisplayed(HostingTypesObjects.DeleteHostingTypeButton)
                .Should()
                .BeFalse();
        }

        public void Dispose()
        {
            ClearHostingTypes(SolutionId);
        }
    }
}
