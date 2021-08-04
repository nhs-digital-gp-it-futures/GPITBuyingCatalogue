using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Marketing.Dashboard
{
    public sealed class Integrations : TestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        public Integrations(LocalWebApplicationFactory factory)
            : base(factory, "marketing/supplier/solution/99999-002/section/integrations")
        {
            using var context = GetEndToEndDbContext();
            var solution = context.Solutions.Single(s => s.Id == new CatalogueItemId(99999, "002"));
            solution.IntegrationsUrl = string.Empty;
            context.SaveChanges();

            AuthorityLogin();
        }

        [Fact]
        public async Task Integrations_AddUrl()
        {
            var link = TextGenerators.UrlInputAddText(Objects.Common.CommonSelectors.LinkTextBox, 1000);

            CommonActions.ClickSave();

            await using var context = GetEndToEndDbContext();
            var solution = await context.Solutions.SingleAsync(s => s.Id == new CatalogueItemId(99999, "002"));
            solution.IntegrationsUrl.Should().Be(link);
        }

        [Fact]
        public void Integrations_SectionMarkedAsComplete()
        {
            Driver.Navigate().Refresh();

            TextGenerators.UrlInputAddText(Objects.Common.CommonSelectors.LinkTextBox, 1000);
            CommonActions.ClickSave();

            MarketingPages.DashboardActions.SectionMarkedComplete("Integrations").Should().BeTrue();
        }

        [Fact]
        public void Integrations_SectionMarkedAsIncomplete()
        {
            Driver.Navigate().Refresh();

            CommonActions.ClickSave();

            MarketingPages.DashboardActions.SectionMarkedComplete("Integrations").Should().BeFalse();
        }

        public void Dispose()
        {
            ClearClientApplication(new CatalogueItemId(99999, "002"));
        }
    }
}
