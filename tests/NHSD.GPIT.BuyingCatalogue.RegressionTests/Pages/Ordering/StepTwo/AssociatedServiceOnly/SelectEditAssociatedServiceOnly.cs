using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering.SolutionSelection;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.SolutionSelection;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering.StepTwo.AssociatedServiceOnly
{
    public class SelectEditAssociatedServiceOnly : PageBase
    {
        public SelectEditAssociatedServiceOnly(IWebDriver driver, CommonActions commonActions)
            : base(driver, commonActions)
        {
        }

        public void SelectAssociatedServices(string solutionName, IEnumerable<string>? associatedServices)
        {
            SelectSolutionForAssociatedService(solutionName);

            if (associatedServices != default && associatedServices.All(a => a != string.Empty))
            {
                foreach (var associatedService in associatedServices)
                {
                    SelectAssociatedServiceOfSolution(associatedService);
                }

                CommonActions.ClickSave();
            }
        }

        public void EditSolutionForAssociatedService(string newSolutionName, IEnumerable<string>? newAssociatedServices)
        {
            CommonActions.ClickLinkElement(ReviewSolutionsObjects.ChangeCatalogueSolutionLink);

            CommonActions.PageLoadedCorrectGetIndex(
             typeof(CatalogueSolutionsController),
             nameof(CatalogueSolutionsController.EditSolutionAssociatedServicesOnly)).Should().BeTrue();

            CommonActions.ClickRadioButtonWithText(newSolutionName);

            CommonActions.ClickSave();

            ConfirmSolutionChanges();

            if (newAssociatedServices != default && newAssociatedServices.All(a => a != string.Empty))
            {
                foreach (var associatedService in newAssociatedServices)
                {
                    SelectAssociatedServiceOfSolution(associatedService);
                }
            }

            CommonActions.ClickSave();
        }

        public void EditAssociatedServiceOnly(IEnumerable<string> newAssociatedServices, IEnumerable<string> oldAssociatedServices)
        {
            CommonActions.ClickLinkElement(ReviewSolutionsObjects.ChangeAssociatedServiceLink);

            CommonActions.PageLoadedCorrectGetIndex(
             typeof(AssociatedServicesController),
             nameof(AssociatedServicesController.SelectAssociatedServices)).Should().BeTrue();

            if (oldAssociatedServices != default && oldAssociatedServices.All(a => a != string.Empty))
            {
                foreach (var oldAssociatedService in oldAssociatedServices)
                {
                    CommonActions.ClickCheckboxByLabel(oldAssociatedService);
                }
            }

            foreach (var newAssociatedService in newAssociatedServices)
            {
                CommonActions.ClickCheckboxByLabel(newAssociatedService);
            }

            CommonActions.ClickSave();

            ConfirmAssociatedServicesChanges();
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
        }
    }
}
