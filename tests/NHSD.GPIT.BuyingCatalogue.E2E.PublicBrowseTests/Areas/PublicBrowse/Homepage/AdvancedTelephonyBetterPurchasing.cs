using FluentAssertions;
using System.IO;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.PublicBrowse;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Utils.Files;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.PublicBrowse.Homepage
{
    [Collection(nameof(SharedContextCollection))]
    public class AdvancedTelephonyBetterPurchasing : AnonymousTestBase
    {
        public AdvancedTelephonyBetterPurchasing(LocalWebApplicationFactory factory)
               : base(factory, typeof(HomeController), nameof(HomeController.AdvancedTelephony))
        {
        }

        [Fact]
        public void AdvancedTelephony_AllSectionsDisplayed()
        {
            CommonActions.PageTitle().Should().Be("Advanced Telephony Better Purchasing framework".FormatForComparison());
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.ElementIsDisplayed(AdvancedTelephonyObjects.HomepageButton).Should().BeTrue();
        }

        [Fact]
        public void AdvancedTelephony_ClickBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(HomeController),
                nameof(HomeController.Index)).Should().BeTrue();
        }

        [Fact]
        public void AdvancedTelephony_ClickHomepageButton_Redirects()
        {
            CommonActions.ClickLinkElement(AdvancedTelephonyObjects.HomepageButton);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(HomeController),
                nameof(HomeController.Index)).Should().BeTrue();
        }

        [Fact]
        public void DownloadCommissioningSupportPackPDF_ExpectedResult()
        {
            var filePath = @$"{Path.GetTempPath()}Buyer's Guide for Advanced Cloud-based Telephony-Jan 2023.pdf";

            FileHelper.DeleteDownloadFile(filePath);

            CommonActions.ClickLinkElement(AdvancedTelephonyObjects.DownloadCommissioningSupportPackPDFButton);

            FileHelper.WaitForDownloadFile(filePath);

            FileHelper.FileExists(filePath).Should().BeTrue();
            FileHelper.FileLength(filePath).Should().BePositive();
            FileHelper.ValidateIsPdf(filePath);

            FileHelper.DeleteDownloadFile(filePath);
        }
    }
}
