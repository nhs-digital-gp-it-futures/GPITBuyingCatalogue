using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Marketing.Dashboard
{
    public sealed class SupplierDetails : TestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public SupplierDetails(LocalWebApplicationFactory factory) : base(factory, "marketing/supplier/solution/99999-99/section/about-supplier")
        {
            using var context = GetBCContext();
            var catalogueItem = context.CatalogueItems.Include(c => c.Supplier).Single(s => s.CatalogueItemId == "99999-99");
            catalogueItem.Supplier.Summary = string.Empty;
            catalogueItem.Supplier.SupplierUrl = string.Empty;
            context.SaveChanges();
        }

        [Fact]
        public async Task SupplierDetails_EnterSummary()
        {
            var summary = MarketingPages.AboutSupplierActions.DescriptionAddText(1000);
            var link = MarketingPages.AboutSupplierActions.LinkAddText(1000);
            MarketingPages.CommonActions.ClickSave();

            using var context = GetBCContext();
            var catalogueItem = await context.CatalogueItems.Include(c => c.Supplier).SingleAsync(s => s.CatalogueItemId == "99999-99");
            catalogueItem.Supplier.Summary.Should().Be(summary);
            catalogueItem.Supplier.SupplierUrl.Should().Be(link);
        }

        [Fact]
        public void SupplierDetails_MarkedAsComplete()
        {
            MarketingPages.AboutSupplierActions.DescriptionAddText(1000);
            MarketingPages.CommonActions.ClickSave();

            MarketingPages.DashboardActions.SectionMarkedComplete("About supplier").Should().BeTrue();
        }

        [Fact]
        public void SupplierDetails_MarkedAsIncomplete()
        {
            MarketingPages.CommonActions.ClickGoBackLink();

            MarketingPages.DashboardActions.SectionMarkedComplete("About supplier").Should().BeFalse();
        }
    }
}
