using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Marketing.ClientApplication.NativeMobile
{
    public sealed class MobileFirst : TestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        public MobileFirst(LocalWebApplicationFactory factory)
            : base(factory, "marketing/supplier/solution/99999-99/section/native-mobile/mobile-first-approach")
        {
        }

        [Theory]
        [InlineData("Yes")]
        [InlineData("No")]
        public async Task MobileFirst_CompleteRadioButton(string label)
        {
            CommonActions.ClickRadioButtonWithText(label);

            CommonActions.ClickSave();

            await using var context = GetEndToEndDbContext();

            var clientApplication = (await context.Solutions.SingleAsync(s => s.Id == new CatalogueItemId(99999, "99"))).ClientApplication;

            clientApplication.Should().ContainEquivalentOf($"NativeMobileFirstDesign\":{(label == "Yes").ToString().ToLower()}");
        }

        [Fact]
        public void MobileFirst_SectionComplete()
        {
            CommonActions.ClickRadioButtonWithText("Yes");

            CommonActions.ClickSave();

            MarketingPages.DashboardActions.SectionMarkedComplete("Mobile first approach").Should().BeTrue();
        }

        [Fact]
        public void MobileFirst_SectionIncomplete()
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
