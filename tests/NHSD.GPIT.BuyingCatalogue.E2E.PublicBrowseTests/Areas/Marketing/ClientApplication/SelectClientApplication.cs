using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using System.Linq;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Marketing.ClientApplication
{
    public sealed class SelectClientApplication : TestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public SelectClientApplication(LocalWebApplicationFactory factory) : base(factory, "marketing/supplier/solution/99999-99/section/client-application-types")
        {
            using var context = GetBCContext();
            var solution = context.Solutions.Single(s => s.Id == "99999-99");
            solution.ClientApplication = string.Empty;
            context.SaveChanges();
        }

        [Theory]
        [InlineData("Browser-based")]
        [InlineData("Native mobile or tablet")]
        [InlineData("Native desktop")]
        public void SelectClientApplication_DashboardSectionEnabled(string clientApplicationType)
        {
            MarketingPages.ClientApplicationTypeActions.SelectClientApplicationCheckbox(clientApplicationType);
            MarketingPages.CommonActions.ClickSave();

            MarketingPages.DashboardActions.SectionDisplayed(clientApplicationType).Should().BeTrue();
        }

        [Fact]
        public void SelectClientApplication_SectionMarkedAsComplete()
        {
            MarketingPages.ClientApplicationTypeActions.SelectClientApplicationCheckbox("Browser-based");
            MarketingPages.CommonActions.ClickSave();

            MarketingPages.DashboardActions.SectionMarkedComplete("Client application type").Should().BeTrue();
        }

        [Fact]
        public void SelectClientApplication_SectionMarkedAsIncomplete()
        {
            MarketingPages.CommonActions.ClickGoBackLink();

            MarketingPages.DashboardActions.SectionMarkedComplete("Client application type").Should().BeFalse();
        }
    }
}
