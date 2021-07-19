using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Marketing;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Marketing.Dashboard
{
    public sealed class ImplementationTimescales : TestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        public ImplementationTimescales(LocalWebApplicationFactory factory)
            : base(factory, "marketing/supplier/solution/99999-99/section/implementation")
        {
            using var context = GetEndToEndDbContext();
            var solution = context.Solutions.Single(s => s.Id == new CatalogueItemId(99999, "99"));
            solution.ImplementationDetail = string.Empty;
            context.SaveChanges();

            AuthorityLogin();
        }

        [Fact]
        public async Task ImplementationTimescales_EnterDescription()
        {
            var implementation = TextGenerators.TextInputAddText(CommonSelectors.Description, 1000);

            CommonActions.ClickSave();

            await using var context = GetEndToEndDbContext();
            var solution = await context.Solutions.SingleAsync(s => s.Id == new CatalogueItemId(99999, "99"));
            solution.ImplementationDetail.Should().Be(implementation);
        }

        [Fact]
        public void ImplementationTimescales_MarkedAsComplete()
        {
            TextGenerators.TextInputAddText(CommonSelectors.Description, 1000);
            CommonActions.ClickSave();

            MarketingPages.DashboardActions.SectionMarkedComplete("Implementation").Should().BeTrue();
        }

        [Fact]
        public void ImplementationTimescales_MarkedAsIncomplete()
        {
            CommonActions.ClickGoBackLink();

            MarketingPages.DashboardActions.SectionMarkedComplete("Implementation").Should().BeFalse();
        }

        public void Dispose()
        {
            ClearClientApplication(new CatalogueItemId(99999, "99"));
        }
    }
}
