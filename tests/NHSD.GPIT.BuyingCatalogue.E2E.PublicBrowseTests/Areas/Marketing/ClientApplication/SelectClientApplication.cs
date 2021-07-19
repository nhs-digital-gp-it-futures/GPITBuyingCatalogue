using System;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Marketing.ClientApplication
{
    public sealed class SelectClientApplication : TestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        public SelectClientApplication(LocalWebApplicationFactory factory)
            : base(factory, "marketing/supplier/solution/99999-99/section/client-application-types")
        {
            AuthorityLogin();
        }

        [Theory]
        [InlineData("Browser-based")]
        [InlineData("Native mobile or tablet")]
        [InlineData("Native desktop")]
        public void SelectClientApplication_DashboardSectionEnabled(string clientApplicationType)
        {
            MarketingPages.ClientApplicationTypeActions.SelectClientApplicationCheckbox(clientApplicationType);
            CommonActions.ClickSave();

            MarketingPages.DashboardActions.SectionDisplayed(clientApplicationType).Should().BeTrue();
        }

        [Fact]
        public void SelectClientApplication_SectionMarkedAsComplete()
        {
            MarketingPages.ClientApplicationTypeActions.SelectClientApplicationCheckbox("Browser-based");
            CommonActions.ClickSave();

            MarketingPages.DashboardActions.SectionMarkedComplete("Client application type").Should().BeTrue();
        }

        [Fact]
        public void SelectClientApplication_SectionMarkedAsIncomplete()
        {
            CommonActions.ClickGoBackLink();

            MarketingPages.DashboardActions.SectionMarkedComplete("Client application type").Should().BeFalse();
        }

        public void Dispose()
        {
            ClearClientApplication(new CatalogueItemId(99999, "99"));
        }
    }
}
