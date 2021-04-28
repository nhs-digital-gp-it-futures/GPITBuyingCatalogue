using NHSD.GPIT.BuyingCatalogue.E2ETests.Common.Actions;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.PublicBrowse
{
    internal sealed class HomepageActions : ActionBase
    {
        public HomepageActions(IWebDriver driver) : base(driver)
        {

        }

        internal bool GpitTileDisplayed()
        {
            try
            {
                Driver.FindElement(Objects.PublicBrowse.HomepageObjects.GpitFrameworkTile);
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
                Driver.FindElement(Objects.PublicBrowse.HomepageObjects.DFOCVCFrameworkTile);
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
                Driver.FindElement(Objects.PublicBrowse.HomepageObjects.BuyersGuideTile);
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
                Driver.FindElement(Objects.PublicBrowse.HomepageObjects.OrderingTile);
                return true;
            }
            catch
            {
                return false;
            }
        }

        internal void ClickDFOCVCTile()
        {
            Driver.FindElement(Objects.PublicBrowse.HomepageObjects.DFOCVCFrameworkTile).Click();
        }

        internal void ClickBuyersGuideTile()
        {
            Driver.FindElement(Objects.PublicBrowse.HomepageObjects.BuyersGuideTile).Click();
        }
    }
}
