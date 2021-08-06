using System.Collections.Generic;
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
    public sealed class AdditionalServicesDetails : AnonymousTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private static readonly CatalogueItemId SolutionId = new(99999, "001");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
        };

        public AdditionalServicesDetails(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(SolutionDetailsController),
                  nameof(SolutionDetailsController.AdditionalServices),
                  Parameters)
        {
        }

        [Fact]
        public async Task AdditionalServicesDetails_AdditionalServicesNameDisplayedAsync()
        {
            await using var context = GetEndToEndDbContext();
            var pageTitle = (await context.CatalogueItems.SingleAsync(s => s.CatalogueItemId == new CatalogueItemId(99999, "001"))).Name;
            CommonActions.PageTitle()
                .Should()
                .BeEquivalentTo($"additional services - {pageTitle}".FormatForComparison());
        }

        [Fact]
        public void AdditionalServicesDetail_AdditionalServicesTableDisplayed()
        {
            PublicBrowsePages.SolutionAction.AdditionalServicesTableDisplayed().Should().BeTrue();
        }

        [Fact]
        public async Task AdditionalServicesDetail_AdditionalServicesListedInTable()
        {
            await using var context = GetEndToEndDbContext();
            var additionalServicesInDb = await context.CatalogueItems
            .Where(c => c.CatalogueItemType == CatalogueItemType.AdditionalService)
            .Where(c => c.SupplierId == "99999")
                .ToListAsync();

            var additionalServicesInTable = PublicBrowsePages.SolutionAction.GetAdditionalServicesNamesFromTable();
            additionalServicesInTable.Should().BeEquivalentTo(additionalServicesInDb.Select(s => s.Name));
        }

        [Fact]
        public async Task AdditionalServicesDetails_AdditionalServicesDescriptionListed()
        {
            await using var context = GetEndToEndDbContext();
            var additionalDescInDb = await context.CatalogueItems
            .Include(c => c.AdditionalService).Where(c => c.CatalogueItemType == CatalogueItemType.AdditionalService)
            .Where(c => c.SupplierId == "99999")
            .Select(a => a.AdditionalService.FullDescription)
            .ToListAsync();

            var additionalDescOnPage = PublicBrowsePages.SolutionAction.GetAdditionalServicesDescription();
            additionalDescOnPage.Should().BeEquivalentTo(additionalDescInDb);
        }
    }
}
