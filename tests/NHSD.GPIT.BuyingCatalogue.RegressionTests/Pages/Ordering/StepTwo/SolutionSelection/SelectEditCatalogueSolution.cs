using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering.Prices;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering.Quantity;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering.SolutionSelection;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers.SolutionSelection;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering.StepTwo.SolutionSelection
{
    public class SelectEditCatalogueSolution : PageBase
    {
        public SelectEditCatalogueSolution(IWebDriver driver, CommonActions commonActions, LocalWebApplicationFactory factory)
            : base(driver, commonActions)
        {
            Factory = factory;
        }

        internal LocalWebApplicationFactory Factory { get; private set; }

        public void SelectSolution(string solutionName, string? additionalService)
        {
            CommonActions.ClickRadioButtonWithText(solutionName);

            if (HasAdditionalService(solutionName) && !string.IsNullOrWhiteSpace(additionalService))
            {
                CommonActions.ClickCheckboxByLabel(additionalService);
            }

            CommonActions.ClickSave();
        }

        public void EditSolution(string newSolutionName, string newAdditionalServiceName)
        {
            CommonActions.ClickLinkElement(ReviewSolutionsObjects.ChangeCatalogueSolutionLink);

            CommonActions.PageLoadedCorrectGetIndex(
             typeof(CatalogueSolutionsController),
             nameof(CatalogueSolutionsController.EditSolution)).Should().BeTrue();

            CommonActions.ClickRadioButtonWithText(newSolutionName);

            CommonActions.ClickSave();

            var hasAdditionalService = HasAdditionalService(newSolutionName);

            ConfirmSolutionChanges(hasAdditionalService, newAdditionalServiceName);
        }

        private void ConfirmSolutionChanges(bool hasAdditionalService, string newAdditionalServiceName)
        {
            CommonActions.PageLoadedCorrectGetIndex(
             typeof(CatalogueSolutionsController),
             nameof(CatalogueSolutionsController.ConfirmSolutionChanges)).Should().BeTrue();

            CommonActions.ClickFirstRadio();

            CommonActions.ClickSave();

            if (hasAdditionalService)
            {
                CommonActions.PageLoadedCorrectGetIndex(
                    typeof(AdditionalServicesController),
                    nameof(AdditionalServicesController.SelectAdditionalServices)).Should().BeTrue();

                if (!string.IsNullOrWhiteSpace(newAdditionalServiceName))
                {
                    CommonActions.ClickCheckboxByLabel(newAdditionalServiceName);
                }

                CommonActions.ClickSave();
            }
        }

        private bool HasAdditionalService(string solutionName)
        {
            using var dbContext = Factory.DbContext;
            return dbContext.AdditionalServices.Any(a => a.Solution.CatalogueItem.Name == solutionName);
        }
    }
}
