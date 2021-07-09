using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Marketing;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Marketing.Dashboard
{
    public sealed class SupplierDetails : TestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        public SupplierDetails(LocalWebApplicationFactory factory)
            : base(factory, "marketing/supplier/solution/99999-99/section/about-supplier")
        {
            using var context = GetEndToEndDbContext();
            var catalogueItem = context.CatalogueItems.Include(c => c.Supplier).Single(s => s.CatalogueItemId == new CatalogueItemId(99999, "99"));
            catalogueItem.Supplier.Summary = string.Empty;
            catalogueItem.Supplier.SupplierUrl = string.Empty;
            context.SaveChanges();

            Login();
        }

        [Fact]
        public async Task SupplierDetails_EnterDescription()
        {
            var summary = TextGenerators.TextInputAddText(CommonSelectors.Description, 1000);
            var link = TextGenerators.UrlInputAddText(CommonSelectors.Link, 1000);

            CommonActions.ClickSave();

            await using var context = GetEndToEndDbContext();
            var catalogueItem = await context.CatalogueItems.Include(c => c.Supplier).SingleAsync(s => s.CatalogueItemId == new CatalogueItemId(99999, "99"));
            catalogueItem.Supplier.Summary.Should().Be(summary);
            catalogueItem.Supplier.SupplierUrl.Should().Be(link);
        }

        [Fact]
        public void SupplierDetails_MarkedAsComplete()
        {
            TextGenerators.TextInputAddText(CommonSelectors.Description, 1000);
            CommonActions.ClickSave();

            MarketingPages.DashboardActions.SectionMarkedComplete("About supplier").Should().BeTrue();
        }

        [Fact]
        public void SupplierDetails_MarkedAsIncomplete()
        {
            CommonActions.ClickGoBackLink();

            MarketingPages.DashboardActions.SectionMarkedComplete("About supplier").Should().BeFalse();
        }

        public void Dispose()
        {
            ClearClientApplication(new CatalogueItemId(99999, "99"));
        }
    }
}
