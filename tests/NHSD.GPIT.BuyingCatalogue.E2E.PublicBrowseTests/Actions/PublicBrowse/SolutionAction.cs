using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.PublicBrowse
{
    internal class SolutionAction : ActionBase
    {
        public SolutionAction(IWebDriver driver) : base(driver)
        {
        }

        internal bool SolutionNameDisplayed()
        {
            return Driver.FindElement(Objects.PublicBrowse.SolutionObjects.SolutionName).Displayed;
        }

        internal void WaitUntilSolutionNameDisplayed()
        {
            Wait.Until(s => SolutionNameDisplayed());
        }

        internal bool FlatListPriceTableDisplayed()
        {
            return ElementDisplayed(Objects.PublicBrowse.SolutionObjects.FlatPriceTable);
        }

        internal string GetTableRowContent(string rowHeader)
        {
            var rows = Driver.FindElements(Objects.PublicBrowse.SolutionObjects.SolutionDetailTableRow);
            var row = rows.Single(s => s.FindElement(By.TagName("dt")).Text.Contains(rowHeader, System.StringComparison.OrdinalIgnoreCase));
            return row.FindElement(By.TagName("dt")).Text;
        }

        internal IEnumerable<string> GetPrices()
        {
            return Driver.FindElement(Objects.PublicBrowse.SolutionObjects.FlatPriceTable)
                .FindElements(By.CssSelector("tbody tr"))
                .Select(s => s.FindElement(Objects.PublicBrowse.SolutionObjects.PriceColumn).Text);
        }

        internal IEnumerable<string> GetSummaryAndDescriptions()
        {
            return Driver.FindElements(Objects.PublicBrowse.SolutionObjects.SummaryAndDescription).Select(s => s.Text);
        }

        internal IEnumerable<string> GetFeatureContent()
        {
            return Driver.FindElement(By.TagName("article"))
                .FindElements(By.TagName("li"))
                .Select(s => s.Text);
        }

        internal string ImplementationNameDisplayed()
        {
            return Driver.FindElement(Objects.PublicBrowse.SolutionObjects.ImplementationName).Text;
        }

        internal IEnumerable<string> GetCapabilitiesContent()
        {
            return Driver.FindElements(Objects.PublicBrowse.SolutionObjects.CapabilitiesContent).Select(s => s.Text);
        }

        internal bool AssociatedServicesTableDisplayed()
        {
            return ElementDisplayed(Objects.PublicBrowse.SolutionObjects.AssociatedServicesTable);
        }

        internal IEnumerable<string> GetAssociationServiesInfo()
        {
            return Driver.FindElements(Objects.PublicBrowse.SolutionObjects.AssociatedServicesInformation).Select(s => s.Text);
        }

        private bool ElementDisplayed(By by)
        {
            try
            {
                Driver.FindElement(by);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
