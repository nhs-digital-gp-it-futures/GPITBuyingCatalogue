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
        public SelectAssociatedServiceOnly(IWebDriver driver, CommonActions commonActions)
            : base(driver, commonActions)
        {
        }

        public void SelectAssociatedServices(string solutionName, string associatedService)
        {
            SelectSolutionForAssociatedService(solutionName);
            SelectAssociatedServiceOfSolution(associatedService);
        }

        private void SelectSolutionForAssociatedService(string solutionName)
        {
            CommonActions.ClickRadioButtonWithText(solutionName);
            CommonActions.ClickSave();
        }

        private void SelectAssociatedServiceOfSolution(string associatedService)
        {
            CommonActions.PageLoadedCorrectGetIndex(
            typeof(AssociatedServicesController),
            nameof(AssociatedServicesController.SelectAssociatedServices))
            .Should().BeTrue();

            //TODO: if there is no assoc service or there is only one

            CommonActions.ClickFirstCheckbox();
            CommonActions.ClickSave();
        }
    }
}
