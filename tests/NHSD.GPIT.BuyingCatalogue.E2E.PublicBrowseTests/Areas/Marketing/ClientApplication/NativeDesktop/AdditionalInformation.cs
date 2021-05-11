using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Marketing.ClientApplication.NativeDesktop
{
    public sealed class AdditionalInformation : TestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public AdditionalInformation(LocalWebApplicationFactory factory) : base(factory, "marketing/supplier/solution/99999-99/section/native-desktop/additional-information")
        {
            ClearClientApplication("99999-99");
            driver.Navigate().Refresh();
        }

        [Fact]
        public async Task AdditionalInformation_CompleteAllFields()
        {
            var additionalInformation = MarketingPages.ClientApplicationTypeActions.EnterAdditionalInformation(500);

            MarketingPages.CommonActions.ClickSave();

            using var context = GetBCContext();

            var clientApplication = (await context.Solutions.SingleAsync(s => s.Id == "99999-99")).ClientApplication;
            clientApplication.Should().ContainEquivalentOf($"\"NativeDesktopAdditionalInformation\":\"{additionalInformation}\"");
        }

        [Fact]
        public void AdditionalInformation_SectionComplete()
        {
            MarketingPages.ClientApplicationTypeActions.EnterAdditionalInformation(500);

            MarketingPages.CommonActions.ClickSave();

            MarketingPages.DashboardActions.SectionMarkedComplete("Additional information").Should().BeTrue();
        }

        [Fact]
        public void AdditionalInformation_SectionIncomplete()
        {
            MarketingPages.CommonActions.ClickGoBackLink();

            MarketingPages.DashboardActions.SectionMarkedComplete("Additional information").Should().BeFalse();
        }
    }
}
