using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers.SolutionSelection;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering.StepTwo.SolutionSelection
{
    public class AssociatedService : PageBase
    {
        public AssociatedService(IWebDriver driver, CommonActions commonActions)
            : base(driver, commonActions)
        {
        }

        public void AddAssociatedService(string preference = "No")
        {
            CommonActions.PageLoadedCorrectGetIndex(
                  typeof(AssociatedServicesController),
                  nameof(AssociatedServicesController.AddAssociatedServices)).Should().BeTrue();

            CommonActions.ClickRadioButtonWithText(preference);

            CommonActions.ClickSave();

            if (preference == "Yes")
            {
                CommonActions.PageLoadedCorrectGetIndex(
                 typeof(AssociatedServicesController),
                 nameof(AssociatedServicesController.SelectAssociatedServices)).Should().BeTrue();

                CommonActions.ClickFirstCheckbox();

                CommonActions.ClickSave();
            }
            else
            {
                CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderController),
                nameof(OrderController.Order)).Should().BeTrue();
            }
        }
    }
}
