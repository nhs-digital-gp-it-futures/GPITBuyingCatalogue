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

        public IList<IWebElement> GetCheckEpicLinks()
        {
            return Driver.FindElements(Objects.PublicBrowse.SolutionObjects.CheckEpicLink);
        }

        public IEnumerable<string> GetCapabilitiesListNames()
        {
            return Driver.FindElements(Objects.PublicBrowse.SolutionObjects.CapabilityName).Select(c => c.Text);
        }

        public string GetTableRowContent(string rowHeader)
        {
            var rows = Driver.FindElements(Objects.PublicBrowse.SolutionObjects.SolutionDetailTableRow);
            var row = rows.Single(s => s.FindElement(By.TagName("dt")).Text.Contains(rowHeader, System.StringComparison.OrdinalIgnoreCase));
            return row.FindElement(By.TagName("dt")).Text;
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

        public IEnumerable<string> GetAdditionalServicesDescription()
        {
            return Driver.FindElements(Objects.PublicBrowse.SolutionObjects.FullDescription).Select(s => s.Text).ToList();
        }
    }
}
