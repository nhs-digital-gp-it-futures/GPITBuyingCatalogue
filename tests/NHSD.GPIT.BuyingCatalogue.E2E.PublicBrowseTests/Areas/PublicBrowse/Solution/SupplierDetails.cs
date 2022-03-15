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
using Xunit.Abstractions;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.PublicBrowse.Solution
{
    public sealed class SupplierDetails : AnonymousTestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        private static readonly CatalogueItemId SolutionId = new(99999, "001");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
        };

        public SupplierDetails(LocalWebApplicationFactory factory, ITestOutputHelper testOutputHelper)
            : base(
                  factory,
                  typeof(SolutionsController),
                  nameof(SolutionsController.SupplierDetails),
                  Parameters,
                  testOutputHelper)
        {
        }

        [Theory]
        [InlineData("Contact name")]
        [InlineData("Contact details")]
        [InlineData("Department")]
        public void SupplierDetails_ContactDetailsDisplayed(string rowHeader)
        {
            RunTest(() =>
            {
                PublicBrowsePages.SolutionAction.GetTableRowContent(rowHeader).Should().NotBeNullOrEmpty();
            });
        }

        [Fact]
        public async Task SupplierDetails_VerifySupplierSummary()
        {
            await RunTestAsync(async () =>
            {
                await using var context = GetEndToEndDbContext();
                var info = (await context.Suppliers.SingleAsync(s => s.Id == 99999)).Summary;
                var supplierInfo = PublicBrowsePages.SolutionAction.GetSummaryAndDescriptions();

                supplierInfo.Should().Contain(info);
            });
        }

        [Fact]
        public async Task SupplierDetails_SolutionIsSuspended_Redirect()
        {
            await RunTestAsync(async () =>
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
            });
        }

        public void Dispose()
        {
            using var context = GetEndToEndDbContext();
            context.CatalogueItems.Single(ci => ci.Id == SolutionId).PublishedStatus = PublicationStatus.Published;
            context.SaveChanges();
        }
    }
}
