using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Marketing.ClientApplication.BrowserBased
{
    public sealed class MobileFirstApproach : TestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        public MobileFirstApproach(LocalWebApplicationFactory factory) : base(factory, "marketing/supplier/solution/99999-99/section/browser-based/mobile-first-approach")
        {
        }

        [Theory]
        [InlineData("Yes")]
        [InlineData("No")]
        public async Task MobileFirstApproach_SelectRadioButton(string label)
        {
            CommonActions.ClickRadioButtonWithText(label);

            CommonActions.ClickSave();

            string labelConvert = label == "Yes" ? "true" : "false";

            await using var context = GetEndToEndDbContext();
            var clientApplication = (await context.Solutions.SingleAsync(s => s.Id == new CatalogueItemId(99999, "99"))).ClientApplication;
            clientApplication.Should().ContainEquivalentOf(@$"MobileFirstDesign"":{ labelConvert }");
        }

        [Fact]
        public void MobileFirstApproach_SectionComplete()
        {
            CommonActions.ClickRadioButtonWithText("Yes");

            CommonActions.ClickSave();

            MarketingPages.DashboardActions.SectionMarkedComplete("Mobile first approach").Should().BeTrue();
        }

        [Fact]
        public void MobileFirstApproach_SectionIncomplete()
        {
            CommonActions.ClickGoBackLink();

            MarketingPages.DashboardActions.SectionMarkedComplete("Mobile first approach").Should().BeFalse();
        }

        public void Dispose()
        {
            ClearClientApplication(new CatalogueItemId(99999, "99"));
        }
    }
}
