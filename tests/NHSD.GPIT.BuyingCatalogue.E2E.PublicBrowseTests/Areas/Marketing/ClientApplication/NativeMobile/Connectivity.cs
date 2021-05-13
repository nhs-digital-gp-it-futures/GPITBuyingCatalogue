using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using System.Threading.Tasks;
using System;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Marketing.ClientApplication.NativeMobile
{
    public sealed class Connectivity : TestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        public Connectivity(LocalWebApplicationFactory factory) : base(factory, "marketing/supplier/solution/99999-99/section/native-mobile/connectivity")
        {
        }

        [Fact]
        public async Task Connectivity_CompleteAllFields()
        {
            CommonActions.SelectDropdownItem(CommonSelectors.ConnectionSpeedSelect, 1);

            CommonActions.ClickFirstCheckbox();

            var description = TextGenerators.TextInputAddText(CommonSelectors.Description, 300);

            CommonActions.ClickSave();

            using var context = GetBCContext();

            var clientApplication = (await context.Solutions.SingleAsync(s => s.Id == "99999-99")).ClientApplication;

            clientApplication.Should().ContainEquivalentOf("ConnectionType\":[\"GPRS\"]");
            clientApplication.Should().ContainEquivalentOf("MinimumConnectionSpeed\":\"0.5Mbps\"");
            clientApplication.Should().ContainEquivalentOf($"Description\":\"{description}\"");
        }

        [Fact]
        public void Connectivity_SectionComplete()
        {
            CommonActions.SelectDropdownItem(CommonSelectors.ConnectionSpeedSelect, 1);

            CommonActions.ClickFirstCheckbox();

            TextGenerators.TextInputAddText(CommonSelectors.Description, 300);

            CommonActions.ClickSave();

            MarketingPages.DashboardActions.SectionMarkedComplete("Connectivity").Should().BeTrue();
        }

        [Fact]
        public void Connectivity_SectionIncomplete()
        {
            CommonActions.ClickGoBackLink();

            MarketingPages.DashboardActions.SectionMarkedComplete("Connectivity").Should().BeFalse();
        }

        public void Dispose()
        {
            ClearClientApplication("99999-99");
        }
    }
}
