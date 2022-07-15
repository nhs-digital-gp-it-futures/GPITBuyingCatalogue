using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering.Prices;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering.Quantity;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers.SolutionSelection;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering.StepTwo.SolutionSelection
{
    public class SelectCatalogueSolution : PageBase
    {
        private const string SolutionName = "NotEmis Web GP";
        private const bool HasAssociatedServices = false;

        public SelectCatalogueSolution(IWebDriver driver, CommonActions commonActions)
            : base(driver, commonActions)
        {
        }

        public void SelectSolution(bool withAdditionalService)
        {
            CommonActions.ClickRadioButtonWithText(SolutionName);

            if (withAdditionalService)
            {
                CommonActions.ClickFirstCheckbox();
            }

            CommonActions.ClickSave();

        }

        public bool ProvidesAssociatedService()
        {
            // check the solution against the db
            return false;
        }
    }
}
