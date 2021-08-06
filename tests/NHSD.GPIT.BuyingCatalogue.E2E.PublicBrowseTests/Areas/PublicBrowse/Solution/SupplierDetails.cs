using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.PublicBrowse.Solution
{
    public sealed class SupplierDetails : AnonymousTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private static readonly CatalogueItemId SolutionId = new(99999, "001");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
        };

        public SupplierDetails(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(SolutionDetailsController),
                  nameof(SolutionDetailsController.SupplierDetails),
                  Parameters)
        {
        }

        [Theory]
        [InlineData("Contact name")]
        [InlineData("Contact details")]
        [InlineData("Department")]
        public void SupplierDetails_ContactDetailsDisplayed(string rowHeader)
        {
            PublicBrowsePages.SolutionAction.GetTableRowContent(rowHeader).Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task SupplierDetails_VerifySupplierSummary()
        {
            await using var context = GetEndToEndDbContext();
            var info = (await context.Suppliers.SingleAsync(s => s.Id == "99999")).Summary;
            var supplierInfo = PublicBrowsePages.SolutionAction.GetSummaryAndDescriptions();

            supplierInfo.Should().Contain(info);
        }
    }
}
