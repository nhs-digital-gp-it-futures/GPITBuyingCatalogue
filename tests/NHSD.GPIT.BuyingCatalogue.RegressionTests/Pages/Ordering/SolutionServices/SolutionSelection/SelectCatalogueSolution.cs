using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers.SolutionSelection;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering.Solution_Services.SolutionSelection
{
    public class SelectCatalogueSolution : PageBase
    {
        private const string SolutionName = "DFOCVC Online Consultation";
        public SelectCatalogueSolution(IWebDriver driver, CommonActions commonActions) 
            : base(driver, commonActions)
        {
        }

        public void SelectCatalogueSolution_SelectSolution()
        {
            CommonActions.ClickRadioButtonWithText(SolutionName);
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ServiceRecipientsController),
                nameof(ServiceRecipientsController.AddServiceRecipients)).Should().BeTrue();
        }
    }
}
