using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.PublicBrowse.Solution
{
    public sealed class SupplierDetails : TestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public SupplierDetails(LocalWebApplicationFactory factory)
            : base(factory, "solutions/futures/99999-001/supplier-details")
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
