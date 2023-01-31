﻿using FluentAssertions;
using System.IO;
using System;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.PublicBrowse;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Utils.Files;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.PublicBrowse.Homepage
{
    public class AdvancedTelephonyBetterPurchasing : AnonymousTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public AdvancedTelephonyBetterPurchasing(LocalWebApplicationFactory factory)
               : base(factory, typeof(HomeController), nameof(HomeController.AdvacedTelephonyBetterPurchaseFramework))
        {
        }

        [Fact]
        public void AdvacedTelephony_AllSectionsDisplayed()
        {
            CommonActions.PageTitle().Should().Be("Advanced Telephony Better Purchasing framework".FormatForComparison());
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.ElementIsDisplayed(AdvancedTelephonyObjects.HomepageButton).Should().BeTrue();
        }

        [Fact]
        public void AdvacedTelephony_ClickBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(HomeController),
                nameof(HomeController.Index)).Should().BeTrue();
        }

        [Fact]
        public void AdvacedTelephony_ClickHomepageButton_Redirects()
        {
            CommonActions.ClickLinkElement(AdvancedTelephonyObjects.HomepageButton);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(HomeController),
                nameof(HomeController.Index)).Should().BeTrue();
        }

        [Fact]
        public void DownloadComissioningSupportPackPDF_ExpectedResult()
        {
            var filePath = @$"{Path.GetTempPath()}Advanced GP Telephony Specification Commissioning Support Pack v1.12.pdf";

            FileHelper.DeleteDownloadFile(filePath);

            CommonActions.ClickLinkElement(AdvancedTelephonyObjects.DownloadComissioningSupportPackPDFButton);

            FileHelper.WaitForDownloadFile(filePath);

            FileHelper.FileExists(filePath).Should().BeTrue();
            FileHelper.FileLength(filePath).Should().BePositive();
            FileHelper.ValidateIsPdf(filePath);

            FileHelper.DeleteDownloadFile(filePath);
        }
    }
}