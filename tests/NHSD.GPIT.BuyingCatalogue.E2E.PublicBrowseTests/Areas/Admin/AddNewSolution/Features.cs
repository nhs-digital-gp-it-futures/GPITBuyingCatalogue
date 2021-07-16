using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.AddNewSolution
{
    public sealed class Features : TestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public Features(LocalWebApplicationFactory factory)
            : base(factory, "/admin/catalogue-solutions/manage/99999-888/features")
        {
            ClearFeatures(new CatalogueItemId(99999, "888"));
            Login();
        }

        [Fact]
        public async Task Features_TitleDisplayedCorrectly()
        {
            await using var context = GetEndToEndDbContext();
            var solutionName = (await context.CatalogueItems.SingleAsync(s => s.CatalogueItemId == new CatalogueItemId(99999, "888"))).Name;

            PublicBrowsePages.CommonActions.PageTitle().Should().BeEquivalentTo($"Features - {solutionName}");
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(6)]
        [InlineData(7)]
        [InlineData(8)]
        [InlineData(9)]
        [InlineData(10)]
        public async Task Features_EnterFeatures(int numFeatures)
        {
            var features = AdminPages.Features.EnterFeatures(numFeatures);

            AdminPages.CommonActions.SavePage();

            await using var context = GetEndToEndDbContext();
            var catalogueItem = await context.CatalogueItems.Include(c => c.Solution).SingleAsync(s => s.CatalogueItemId == new CatalogueItemId(99999, "888"));
            var featuresModel = new FeaturesModel().FromCatalogueItem(catalogueItem);

            featuresModel.AllFeatures.Should().BeEquivalentTo(features);
        }

        [Fact]
        public async Task Features_GoBackWithoutSaving()
        {
            AdminPages.Features.EnterFeatures(10);

            AdminPages.CommonActions.ClickGoBack();

            await using var context = GetEndToEndDbContext();
            var solution = await context.Solutions.SingleAsync(s => s.Id == new CatalogueItemId(99999, "888"));
            var features = solution.Features;

            features.Should().BeNullOrEmpty();
        }
    }
}
