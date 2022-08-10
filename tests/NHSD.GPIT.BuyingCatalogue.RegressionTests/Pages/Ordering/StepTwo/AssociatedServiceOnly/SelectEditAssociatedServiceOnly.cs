using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering.SolutionSelection;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers.SolutionSelection;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering.StepTwo.AssociatedServiceOnly
{
    public class SelectEditAssociatedServiceOnly : PageBase
    {
        public SelectEditAssociatedServiceOnly(IWebDriver driver, CommonActions commonActions)
            : base(driver, commonActions)
        {
        }

        public void SelectAssociatedServices(string solutionName, string associatedService)
        {
            SelectSolutionForAssociatedService(solutionName);
            SelectAssociatedServiceOfSolution(associatedService);
        }

        public void EditSolutionForAssociatedService(string newSolutionName, string newAssociatedService)
        {
            CommonActions.ClickLinkElement(ReviewSolutionsObjects.ChangeCatalogueSolutionLink);

            CommonActions.PageLoadedCorrectGetIndex(
             typeof(CatalogueSolutionsController),
             nameof(CatalogueSolutionsController.EditSolutionAssociatedServicesOnly)).Should().BeTrue();

            CommonActions.ClickRadioButtonWithText(newSolutionName);

            CommonActions.ClickSave();

            ConfirmSolutionChanges();

            SelectAssociatedServiceOfSolution(newAssociatedService);
        }

        private void ConfirmSolutionChanges()
        {
            CommonActions.PageLoadedCorrectGetIndex(
             typeof(CatalogueSolutionsController),
             nameof(CatalogueSolutionsController.ConfirmSolutionChangesAssociatedServicesOnly)).Should().BeTrue();

            CommonActions.ClickFirstRadio();

            CommonActions.ClickSave();
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

            CommonActions.ClickCheckboxByLabel(associatedService);
            CommonActions.ClickSave();
        }
    }
}
