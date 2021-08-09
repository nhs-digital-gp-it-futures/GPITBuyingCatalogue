﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.PublicBrowse.Solution
{
    public sealed class AssociatedServicesDetails : AnonymousTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private static readonly CatalogueItemId SolutionId = new(99999, "001");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
        };

        public AssociatedServicesDetails(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(SolutionDetailsController),
                  nameof(SolutionDetailsController.AssociatedServices),
                  Parameters)
        {
        }

        [Fact]
        public async Task AssociatedServicesDetails_AssociatedServicesNameDisplayedAsync()
        {
            await using var context = GetEndToEndDbContext();
            var pageTitle = (await context.CatalogueItems.SingleAsync(s => s.Id == new CatalogueItemId(99999, "001"))).Name;

            CommonActions
                .PageTitle()
                .Should()
                .BeEquivalentTo($"associated services - {pageTitle}".FormatForComparison());
        }

        [Fact]
        public void AssociatedServicesDetails_AssociatedServicesTableDisplayed()
        {
            PublicBrowsePages.SolutionAction.AssociatedServicesTableDisplayed().Should().BeTrue();
        }

        [Fact]
        public async Task AssociatedServicesDetails_AssociatedServicesListedInTable()
        {
            await using var context = GetEndToEndDbContext();

            var associatedServicesInDb = await context.CatalogueItems.Where(c => c.CatalogueItemType == CatalogueItemType.AssociatedService).Where(c => c.SupplierId == "99999").ToListAsync();

            var associatedServicesInTable = PublicBrowsePages.SolutionAction.GetAssociatedServicesNamesFromTable();

            associatedServicesInTable.Should().BeEquivalentTo(associatedServicesInDb.Select(s => s.Name));
        }

        [Fact]
        public async Task AssociatedServicesDetails_AssociatedServicesDetailsListed()
        {
            await using var context = GetEndToEndDbContext();

            var associatedServicesInDb = await context.CatalogueItems.Include(s => s.AssociatedService).Where(c => c.CatalogueItemType == CatalogueItemType.AssociatedService).Where(c => c.SupplierId == "99999").ToListAsync();

            var associatedServicesOnPage = PublicBrowsePages.SolutionAction.GetAssociatedServicesInfo();

            associatedServicesOnPage.Should().BeEquivalentTo(
                associatedServicesInDb.Select(s => s.AssociatedService),
                options =>
                    options.Including(s => s.Description)
                    .Including(s => s.OrderGuidance));
        }
    }
}
