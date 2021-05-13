using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using System.Threading.Tasks;
using System;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Marketing.ClientApplication.NativeDesktop
{
    public sealed class AdditionalInformation : TestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        public AdditionalInformation(LocalWebApplicationFactory factory) : base(factory, "marketing/supplier/solution/99999-99/section/native-desktop/additional-information")
        {
        }

        [Fact]
        public async Task AdditionalInformation_CompleteAllFields()
        {
            var additionalInformation = TextGenerators.TextInputAddText(CommonSelectors.AdditionalInfoTextArea, 500);

            CommonActions.ClickSave();

            using var context = GetBCContext();

            var clientApplication = (await context.Solutions.SingleAsync(s => s.Id == "99999-99")).ClientApplication;
            clientApplication.Should().ContainEquivalentOf($"\"NativeDesktopAdditionalInformation\":\"{additionalInformation}\"");
        }

        [Fact]
        public void AdditionalInformation_SectionComplete()
        {
            TextGenerators.TextInputAddText(CommonSelectors.AdditionalInfoTextArea, 500);

            CommonActions.ClickSave();

            MarketingPages.DashboardActions.SectionMarkedComplete("Additional information").Should().BeTrue();
        }

        [Fact]
        public void AdditionalInformation_SectionIncomplete()
        {
            CommonActions.ClickGoBackLink();

            MarketingPages.DashboardActions.SectionMarkedComplete("Additional information").Should().BeFalse();
        }

        public void Dispose()
        {
            ClearClientApplication("99999-99");
        }
    }
}
