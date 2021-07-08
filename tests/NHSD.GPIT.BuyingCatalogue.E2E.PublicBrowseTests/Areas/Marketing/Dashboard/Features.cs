using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Marketing.Dashboard
{
    public sealed class Features : TestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        public Features(LocalWebApplicationFactory factory)
            : base(factory, "marketing/supplier/solution/99999-99/section/features")
        {
        }

        [Fact]
        public async Task Features_AddFeaturesAsync()
        {
            var feature = MarketingPages.FeaturesActions.EnterFeature();

            CommonActions.ClickSave();

            await using var context = GetEndToEndDbContext();
            var solution = await context.Solutions.SingleAsync(s => s.Id == new CatalogueItemId(99999, "99"));

            solution.Features.Should().ContainEquivalentOf(feature);
        }

        [Fact]
        public async Task Features_Add10FeaturesAsync()
        {
            List<string> addedFeatures = new();

            for (int i = 0; i < 10; i++)
            {
                var feature = MarketingPages.FeaturesActions.EnterFeature(i);
                addedFeatures.Add(feature);
            }

            CommonActions.ClickSave();

            await using var context = GetEndToEndDbContext();
            var solution = await context.Solutions.SingleAsync(s => s.Id == new CatalogueItemId(99999, "99"));

            foreach (var feature in addedFeatures)
            {
                solution.Features.Should().ContainEquivalentOf(feature);
            }
        }

        [Fact]
        public void Features_SectionMarkedComplete()
        {
            MarketingPages.FeaturesActions.EnterFeature();

            CommonActions.ClickSave();

            MarketingPages.DashboardActions.SectionMarkedComplete("Features").Should().BeTrue();
        }

        [Fact]
        public void Features_SectionMarkedIncomplete()
        {
            CommonActions.ClickSave();

            MarketingPages.DashboardActions.SectionMarkedComplete("Features").Should().BeFalse();
        }

        public void Dispose()
        {
            ClearFeatures(new CatalogueItemId(99999, "99"));
        }
    }
}
