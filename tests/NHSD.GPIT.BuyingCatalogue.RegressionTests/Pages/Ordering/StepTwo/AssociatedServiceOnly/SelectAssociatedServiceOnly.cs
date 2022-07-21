using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers.SolutionSelection;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering.StepTwo.AssociatedServiceOnly
{
    public class SelectAssociatedServiceOnly : PageBase
    {
        private const string SolutionName = "Anywhere Consult";

        public SelectAssociatedServiceOnly(IWebDriver driver, CommonActions commonActions)
            : base(driver, commonActions)
        {
        }

        public void SelectAssociatedServices()
        {
            SelectSolutionForAssociatedService();
            SelectAssociatedServiceOfSolution();
        }

        private void SelectSolutionForAssociatedService()
        {
            CommonActions.ClickRadioButtonWithText(SolutionName);
            CommonActions.ClickSave();
        }

        private void SelectAssociatedServiceOfSolution()
        {
            CommonActions.PageLoadedCorrectGetIndex(
            typeof(AssociatedServicesController),
            nameof(AssociatedServicesController.SelectAssociatedServices))
            .Should().BeTrue();

            CommonActions.ClickFirstCheckbox();
            CommonActions.ClickSave();
        }
    }
}
