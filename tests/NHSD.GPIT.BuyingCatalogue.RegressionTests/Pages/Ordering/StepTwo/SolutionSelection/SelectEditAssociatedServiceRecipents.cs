using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering.SolutionSelection;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.SolutionSelection;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering.StepTwo.SolutionSelection
{
    public class SelectEditAssociatedServiceRecipents : PageBase
    {
        public SelectEditAssociatedServiceRecipents(IWebDriver driver, CommonActions commonActions, LocalWebApplicationFactory factory)
            : base(driver, commonActions)
        {
            Factory = factory;
        }

        public LocalWebApplicationFactory Factory { get; }

        public void AddServiceRecipient()
        {
            CommonActions.PageLoadedCorrectGetIndex(
                 typeof(ServiceRecipientsController),
                 nameof(ServiceRecipientsController.SelectServiceRecipients)).Should().BeTrue();

            CommonActions.ClickSave();
        }

        public void EditServiceRecipient(string associatedServiceName)
        {
            CommonActions.PageLoadedCorrectGetIndex(
              typeof(ServiceRecipientsController),
              nameof(ServiceRecipientsController.SelectServiceRecipients)).Should().BeTrue();

            CommonActions.ClickFirstCheckbox();

            CommonActions.ClickCheckboxByLabel("BEECHWOOD MEDICAL CENTRE");

            CommonActions.ClickSave();
        }

        private string GetAssociatedServiceID(string associatedServiceName)
        {
            using var dbContext = Factory.DbContext;

            var solution = dbContext.AssociatedServices.FirstOrDefault(i => i.CatalogueItem.Name == associatedServiceName);

            return (solution != null) ? solution.CatalogueItemId.ToString() : string.Empty;
        }
    }
}
