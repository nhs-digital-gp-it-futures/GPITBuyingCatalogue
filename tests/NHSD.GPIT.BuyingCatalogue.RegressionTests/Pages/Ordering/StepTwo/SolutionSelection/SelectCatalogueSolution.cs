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
        public SelectCatalogueSolution(IWebDriver driver, CommonActions commonActions, LocalWebApplicationFactory factory)
            : base(driver, commonActions)
        {
            Factory = factory;
        }

        internal LocalWebApplicationFactory Factory { get; private set; }

        public void SelectSolution(string solutionName, string? additionalService)
        {
            using var dbContext = Factory.DbContext;

            var hasAdditionalService = dbContext.AdditionalServices.Any(a => a.Solution.CatalogueItem.Name == solutionName);

            CommonActions.ClickRadioButtonWithText(solutionName);

            if (hasAdditionalService && !string.IsNullOrWhiteSpace(additionalService))
            {
                CommonActions.ClickCheckboxByLabel(additionalService);
            }

            CommonActions.ClickSave();
        }
    }
}
