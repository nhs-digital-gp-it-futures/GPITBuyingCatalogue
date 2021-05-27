using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Marketing;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Marketing.Dashboard
{
    // TODO: fix when page updated with new layout
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1000:Test classes must be public", Justification = "Disabled")]
    internal sealed class Integrations : TestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        public Integrations(LocalWebApplicationFactory factory) : base(factory, "marketing/supplier/solution/99999-99/section/integrations")
        {
            using var context = GetBCContext();
            var solution = context.Solutions.Single(s => s.Id == "99999-99");
            solution.IntegrationsUrl = string.Empty;
            context.SaveChanges();
        }

        [Fact]
        public async Task Integrations_AddUrl()
        {
            var link = TextGenerators.UrlInputAddText(CommonSelectors.Link, 1000);

            CommonActions.ClickSave();

            using var context = GetBCContext();
            var solution = await context.Solutions.SingleAsync(s => s.Id == "99999-99");
            solution.IntegrationsUrl.Should().Be(link);
        }

        [Fact]
        public void Integrations_SectionMarkedAsComplete()
        {
            driver.Navigate().Refresh();

            TextGenerators.UrlInputAddText(CommonSelectors.Link, 1000);
            CommonActions.ClickSave();

            MarketingPages.DashboardActions.SectionMarkedComplete("Integrations").Should().BeTrue();
        }

        [Fact]
        public void Integrations_SectionMarkedAsIncomplete()
        {
            driver.Navigate().Refresh();

            CommonActions.ClickSave();

            MarketingPages.DashboardActions.SectionMarkedComplete("Integrations").Should().BeFalse();
        }

        public void Dispose()
        {
            ClearClientApplication("99999-99");
        }
    }
}
