using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering.SolutionSelection;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.SolutionSelection;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering.StepTwo.SolutionSelection
{
    public class SelectEditAssociatedService : PageBase
    {
        public SelectEditAssociatedService(IWebDriver driver, CommonActions commonActions, LocalWebApplicationFactory factory)
            : base(driver, commonActions)
        {
            Factory = factory;
        }

        internal LocalWebApplicationFactory Factory { get; private set; }

        public void AddAssociatedService(string preference = "No", string associatedService = "")
        {
            CommonActions.ClickRadioButtonWithText(preference);

            CommonActions.ClickSave();

            if (preference == "Yes")
            {
                CommonActions.PageLoadedCorrectGetIndex(
                 typeof(AssociatedServicesController),
                 nameof(AssociatedServicesController.SelectAssociatedServices)).Should().BeTrue();

                CommonActions.ClickCheckboxByLabel(associatedService);

                CommonActions.ClickSave();
            }
        }

        public void AddAssociatedService(IEnumerable<string>? associatedServices, string preference = "No")
        {
            CommonActions.ClickRadioButtonWithText(preference);

            CommonActions.ClickSave();

            if (preference == "Yes")
            {
                CommonActions.PageLoadedCorrectGetIndex(
                 typeof(AssociatedServicesController),
                 nameof(AssociatedServicesController.SelectAssociatedServices)).Should().BeTrue();

                if (associatedServices != default && associatedServices.All(a => !string.IsNullOrWhiteSpace(a)))
                {
                    foreach (var associatedService in associatedServices)
                    {
                        CommonActions.ClickCheckboxByLabel(associatedService);
                    }
                }

                CommonActions.ClickSave();
            }
        }

        public void AddAssociatedService(IEnumerable<string>? associatedServices)
        {
            CommonActions.ClickLinkElement(E2ETests.Framework.Objects.Ordering.AssociatedServicesObjects.AddAssociateServiceLink);

            CommonActions.PageLoadedCorrectGetIndex(
                 typeof(AssociatedServicesController),
                 nameof(AssociatedServicesController.EditAssociatedServices)).Should().BeTrue();

            if (associatedServices != default && associatedServices.All(a => !string.IsNullOrWhiteSpace(a)))
            {
                    foreach (var associatedService in associatedServices)
                    {
                        CommonActions.ClickCheckboxByLabel(associatedService);
                    }
            }

            CommonActions.ClickSave();
        }

        public void EditAssociatedService(string solutionName, IEnumerable<string> newAssociatedServices, bool hasTheOrderAssociatedService, IEnumerable<string>? oldAssociatedServices)
        {
            if (SolutionHasAssociatedService(solutionName))
            {
                if (hasTheOrderAssociatedService)
                {
                    CommonActions.ClickLinkElement(ReviewSolutionsObjects.ChangeAssociatedServiceLink);

                    CommonActions.PageLoadedCorrectGetIndex(
                     typeof(AssociatedServicesController),
                     nameof(AssociatedServicesController.EditAssociatedServices)).Should().BeTrue();

                    if (oldAssociatedServices != default && oldAssociatedServices.All(a => !string.IsNullOrWhiteSpace(a)))
                    {
                        foreach (var oldAssociatedService in oldAssociatedServices)
                        {
                            CommonActions.ClickCheckboxByLabel(oldAssociatedService);
                        }
                    }
                }
                else
                {
                    CommonActions.ClickLinkElement(ReviewSolutionsObjects.AddAssociatedServiceLink);

                    CommonActions.PageLoadedCorrectGetIndex(
                     typeof(AssociatedServicesController),
                     nameof(AssociatedServicesController.EditAssociatedServices)).Should().BeTrue();
                }

                foreach (var associatedService in newAssociatedServices)
                {
                    CommonActions.ClickCheckboxByLabel(associatedService);
                }

                CommonActions.ClickSave();

                if (hasTheOrderAssociatedService)
                    ConfirmAssociatedServiceChanges();
            }
            else
            {
                CommonActions.ClickContinue();

                CommonActions.PageLoadedCorrectGetIndex(
                  typeof(OrderController),
                  nameof(OrderController.Order)).Should().BeTrue();
            }
        }

        private void ConfirmAssociatedServiceChanges()
        {
            CommonActions.PageLoadedCorrectGetIndex(
                  typeof(AssociatedServicesController),
                  nameof(AssociatedServicesController.ConfirmAssociatedServiceChanges)).Should().BeTrue();

            CommonActions.ClickFirstRadio();

            CommonActions.ClickSave();
        }

        private bool SolutionHasAssociatedService(string solutionName)
        {
            using var dbContext = Factory.DbContext;
            var result = dbContext.AssociatedServices.Any(a => a.CatalogueItem.Solution.CatalogueItem.Name == solutionName);
            return result;
        }
    }
}
