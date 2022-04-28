using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.PublicBrowse
{
    public class SolutionAction : ActionBase
    {
        public SolutionAction(IWebDriver driver)
            : base(driver)
        {
        }

        public void ClickEpics()
        {
            Driver.FindElement(Objects.PublicBrowse.SolutionObjects.CheckEpicLink).Click();
        }

        public bool CatalogueSolutionPageDisplayed()
        {
            Driver.FindElement(Objects.PublicBrowse.SolutionObjects.CatalogueSolutionPage);
            return true;
        }

        public bool SolutionNameDisplayed()
        {
            return Driver.FindElement(Objects.PublicBrowse.SolutionObjects.SolutionName).Displayed;
        }

        public IList<IWebElement> GetCheckEpicLinks()
        {
            return Driver.FindElements(Objects.PublicBrowse.SolutionObjects.CheckEpicLink);
        }

        public IEnumerable<string> GetCapabilitiesListNames()
        {
            return Driver.FindElements(Objects.PublicBrowse.SolutionObjects.CapabilityName).Select(c => c.Text);
        }

        public void WaitUntilSolutionNameDisplayed()
        {
            Wait.Until(s => SolutionNameDisplayed());
        }

        public bool FlatListPriceTableDisplayed()
        {
            return ElementDisplayed(Objects.PublicBrowse.SolutionObjects.FlatPriceTable);
        }

        public string GetTableRowContent(string rowHeader)
        {
            var rows = Driver.FindElements(Objects.PublicBrowse.SolutionObjects.SolutionDetailTableRow);
            var row = rows.Single(s => s.FindElement(By.TagName("dt")).Text.Contains(rowHeader, System.StringComparison.OrdinalIgnoreCase));
            return row.FindElement(By.TagName("dt")).Text;
        }

        public IEnumerable<string> GetPrices()
        {
            return Driver.FindElement(Objects.PublicBrowse.SolutionObjects.FlatPriceTable)
                .FindElements(By.CssSelector("tbody tr"))
                .Select(s => s.FindElement(Objects.PublicBrowse.SolutionObjects.PriceColumn).Text.Split("£")[1]);
        }

        public IEnumerable<string> GetSummaryAndDescriptions()
        {
            return Driver.FindElements(Objects.PublicBrowse.SolutionObjects.SummaryAndDescription).Select(s => s.Text);
        }

        public IEnumerable<string> GetFeatureContent()
        {
            return Driver.FindElement(By.TagName("article"))
                .FindElements(By.TagName("li"))
                .Select(s => s.Text);
        }

        public string ImplementationNameDisplayed()
        {
            return Driver.FindElement(Objects.PublicBrowse.SolutionObjects.ImplementationName).Text;
        }

        public IEnumerable<string> GetCapabilitiesContent()
        {
            return Driver.FindElements(Objects.PublicBrowse.SolutionObjects.CapabilitiesContent).Select(s => s.Text);
        }

        public IEnumerable<string> GetNhsSolutionEpics()
        {
            return Driver.FindElements(Objects.PublicBrowse.SolutionObjects.NhsSolutionEpics).Select(s => s.Text);
        }

        public IEnumerable<string> GetSupplierSolutionEpics()
        {
            return Driver.FindElements(Objects.PublicBrowse.SolutionObjects.SupplierSolutionEpics).Select(s => s.Text);
        }

        public bool AssociatedServicesTableDisplayed()
        {
            return ElementDisplayed(Objects.PublicBrowse.SolutionObjects.AssociatedServicesTable);
        }

        public IEnumerable<string> GetAssociatedServicesNamesFromTable()
        {
            return Driver.FindElement(Objects.PublicBrowse.SolutionObjects.AssociatedServicesTable).FindElements(By.TagName("a")).Select(s => s.Text);
        }

        public IEnumerable<AssociatedService> GetAssociatedServicesInfo()
        {
            var associatedServices = new List<AssociatedService>();

            var associatedServicesOnPage = Driver.FindElement(Objects.PublicBrowse.SolutionObjects.AssociatedServicesInformation).FindElements(By.TagName("dl"));

            foreach (var assocServ in associatedServicesOnPage)
            {
                associatedServices.Add(
                    new()
                    {
                        Description = assocServ.FindElement(Objects.PublicBrowse.SolutionObjects.Description).Text,
                        OrderGuidance = assocServ.FindElement(Objects.PublicBrowse.SolutionObjects.OrderGuidance).Text,
                    });
            }

            return associatedServices;
        }

        public string AdditionalServicesNameDisplayed()
        {
            return Driver.FindElement(Objects.PublicBrowse.SolutionObjects.ImplementationName).Text;
        }

        public bool AdditionalServicesTableDisplayed()
        {
            return ElementDisplayed(Objects.PublicBrowse.SolutionObjects.AdditionalServicesTable);
        }

        public IEnumerable<string> GetAdditionalServicesNamesFromTable()
        {
            return Driver.FindElement(Objects.PublicBrowse.SolutionObjects.AdditionalServicesTable).FindElements(By.TagName("a")).Select(s => s.Text);
        }

        public IEnumerable<string> GetAdditionalServicesDescription()
        {
            var additionalServicesOnPage = Driver.FindElements(Objects.PublicBrowse.SolutionObjects.FullDescription);
            return additionalServicesOnPage.Select(e => e.FindElement(By.TagName("dd")).Text).ToList();
        }

        public bool BreadcrumbBannerDisplayed()
        {
            return ElementDisplayed(Objects.PublicBrowse.SolutionObjects.BreadcrumbsBanner);
        }

        public string GetBreadcrumbNames(string breadcrumbItem)
        {
            var rows = Driver.FindElements(Objects.PublicBrowse.SolutionObjects.BreadcrumbsBanner);
            var row = rows.Single(r => r.FindElement(By.TagName("a")).Text.Contains(breadcrumbItem, System.StringComparison.OrdinalIgnoreCase));
            return row.FindElement(By.TagName("a")).Text;
        }

        public void ClickCatalogueSolutionBreadcrumb()
        {
            Driver.FindElement(Objects.PublicBrowse.SolutionObjects.CatalogueSolutionCrumb).Click();
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
