﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.PublicBrowse.Solution
{
    public sealed class FeatureDetails : AnonymousTestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        private static readonly CatalogueItemId SolutionId = new(99999, "001");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
        };

        public FeatureDetails(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(SolutionsController),
                  nameof(SolutionsController.Features),
                  Parameters)
        {
        }

        [Fact]
        public async Task FeatureDetails_VerifyFeatureContent()
        {
            {
                await using var context = GetEndToEndDbContext();
                var featureInfo = (await context.Solutions.SingleAsync(s => s.CatalogueItemId == new CatalogueItemId(99999, "001"))).Features;

                var featureList = PublicBrowsePages.SolutionAction.GetFeatureContent();

                var dbList = featureInfo.Replace("[", string.Empty).Replace("\"", string.Empty).Replace("]", string.Empty).Split(',').ToList().Select(s => s.Trim());
                featureList.Should().BeEquivalentTo(dbList);
            }
        }

        [Fact]
        public async Task FeatureDetails_SolutionIsSuspended_Redirect()
        {
            await using var context = GetEndToEndDbContext();
            var solution = await context.CatalogueItems.SingleAsync(ci => ci.Id == SolutionId);
            solution.PublishedStatus = PublicationStatus.Suspended;
            await context.SaveChangesAsync();

            Driver.Navigate().Refresh();

            CommonActions
                .PageLoadedCorrectGetIndex(
                    typeof(SolutionsController),
                    nameof(SolutionsController.Description))
                .Should()
                .BeTrue();
        }

        public void Dispose()
        {
            using var context = GetEndToEndDbContext();
            context.CatalogueItems.Single(ci => ci.Id == SolutionId).PublishedStatus = PublicationStatus.Published;
            context.SaveChanges();
        }
    }
}
