using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering.SolutionSelection;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.SolutionSelection;
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

        public void SelectSolution(string solutionName, IEnumerable<string>? additionalServices)
        {
            CommonActions.ClickRadioButtonWithText(solutionName);
            CommonActions.ClickSave();
        }

        public void EditSolution(string newSolutionName, IEnumerable<string>? newAdditionalServiceNames)
        {
            CommonActions.ClickLinkElement(ReviewSolutionsObjects.ChangeCatalogueSolutionLink);

            CommonActions.PageLoadedCorrectGetIndex(
             typeof(CatalogueSolutionsController),
             nameof(CatalogueSolutionsController.EditSolution)).Should().BeTrue();

            CommonActions.ClickRadioButtonWithText(newSolutionName);

            CommonActions.ClickSave();
            ConfirmSolutionChanges();
        }

        public void EditAdditionalService(string solutionName, IEnumerable<string>? oldAdditionalServices, IEnumerable<string>? newAdditionalServices, bool hasTheOrderAdditionalService)
        {
            if (SolutionHasAdditionalService(solutionName))
            {
                if (hasTheOrderAdditionalService)
                {
                    CommonActions.ClickLinkElement(ReviewSolutionsObjects.ChangeAdditionalServiceLink);

                    CommonActions.PageLoadedCorrectGetIndex(
                      typeof(AdditionalServicesController),
                      nameof(AdditionalServicesController.SelectAdditionalServices)).Should().BeTrue();

                    if (oldAdditionalServices != default && oldAdditionalServices.All(a => !string.IsNullOrWhiteSpace(a)))
                    {
                        foreach (var oldAdditionalService in oldAdditionalServices)
                        {
                            CommonActions.ClickCheckboxByLabel(oldAdditionalService);
                        }
                    }
                }
                else
                {
                    CommonActions.ClickLinkElement(ReviewSolutionsObjects.AddAdditionalServiceLink);

                    CommonActions.PageLoadedCorrectGetIndex(
                      typeof(AdditionalServicesController),
                      nameof(AdditionalServicesController.SelectAdditionalServices)).Should().BeTrue();
                }

                if (newAdditionalServices != default && newAdditionalServices.All(a => !string.IsNullOrWhiteSpace(a)))
                {
                    foreach (var newAdditionalService in newAdditionalServices)
                    {
                        CommonActions.ClickCheckboxByLabel(newAdditionalService);
                    }
                }

                CommonActions.ClickSave();
            }
            else
            {
                CommonActions.ClickContinue();

                CommonActions.PageLoadedCorrectGetIndex(
                  typeof(OrderController),
                  nameof(OrderController.Order)).Should().BeTrue();
            }
        }

        public void AddAdditionalServices(string additionalService)
        {
            CommonActions.ClickLinkElement(ReviewSolutionsObjects.AddAdditionalServiceLink);
            CommonActions.PageLoadedCorrectGetIndex(
                      typeof(AdditionalServicesController),
                      nameof(AdditionalServicesController.SelectAdditionalServices)).Should().BeTrue();

            CommonActions.ClickCheckboxByLabel(additionalService);
            CommonActions.ClickSave();
        }

        public void AddAssociatedServices(string associatedServices)
        {
            CommonActions.ClickLinkElement(ReviewSolutionsObjects.AddAssociatedServiceLink);
            CommonActions.PageLoadedCorrectGetIndex(
                 typeof(AssociatedServicesController),
                 nameof(AssociatedServicesController.SelectAssociatedServices)).Should().BeTrue();

            CommonActions.ClickCheckboxByLabel(associatedServices);
            CommonActions.ClickSave();
        }

        private void ConfirmSolutionChanges()
        {
            CommonActions.PageLoadedCorrectGetIndex(
             typeof(CatalogueSolutionsController),
             nameof(CatalogueSolutionsController.ConfirmSolutionChanges)).Should().BeTrue();

            CommonActions.ClickFirstRadio();

            CommonActions.ClickSave();
        }

        private bool SolutionHasAdditionalService(string solutionName)
        {
            using var dbContext = Factory.DbContext;
            return dbContext.AdditionalServices.Any(a => a.Solution.CatalogueItem.Name == solutionName);
        }
    }
}
