using OpenQA.Selenium;
using System;

namespace NHSD.GPIT.BuyingCatalogue.E2E.PublicBrowseTests.Actions
{
    internal sealed class HomepageActions : ActionBase
    {
        public HomepageActions(IWebDriver driver) : base (driver)
        {

        }

        internal bool GpitTileDisplayed()
        {
            try
            {
                Driver.FindElement(Objects.HomepageObjects.GpitFrameworkTile);
                return true;
            }
            catch
            {
                return false;
            }
        }

        internal bool DFOCVCTileDisplayed()
        {
            try
            {
                Driver.FindElement(Objects.HomepageObjects.DFOCVCFrameworkTile);
                return true;
            }
            catch
            {
                return false;
            }
        }

        internal bool BuyersGuideTileDisplayed()
        {
            try
            {
                Driver.FindElement(Objects.HomepageObjects.BuyersGuideTile);
                return true;
            }
            catch
            {
                return false;
            }
        }

        internal bool OrderFormTileDisplayed()
        {
            try
            {
                Driver.FindElement(Objects.HomepageObjects.OrderingTile);
                return true;
            }
            catch
            {
                return false;
            }
        }

        internal void ClickDFOCVCTile()
        {
            Driver.FindElement(Objects.HomepageObjects.DFOCVCFrameworkTile).Click();
        }

        internal void ClickBuyersGuideTile()
        {
            Driver.FindElement(Objects.HomepageObjects.BuyersGuideTile).Click();
        }
    }
}
