using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering.SolutionSelection;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.SolutionSelection;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering.StepTwo.AssociatedServiceOnly
{
    public class SelectEditAssociatedServiceRecipientOnly : PageBase
    {
        public SelectEditAssociatedServiceRecipientOnly(IWebDriver driver, CommonActions commonActions, LocalWebApplicationFactory factory)
            : base(driver, commonActions)
        {
            Factory = factory;
        }

        public LocalWebApplicationFactory Factory { get; }

        public void AddServiceRecipient(bool multipleServiceRecipients = false)
        {
            CommonActions.PageLoadedCorrectGetIndex(
              typeof(ServiceRecipientsController),
              nameof(ServiceRecipientsController.AddServiceRecipients)).Should().BeTrue();

            if (multipleServiceRecipients)
            {
                if (!CommonActions.AllCheckBoxesSelected())
                    CommonActions.ClickAllCheckboxes();

                CommonActions.ClickSave();
                return;
            }

            if (CommonActions.GetNumberOfSelectedCheckBoxes() == 0)
                CommonActions.ClickFirstCheckbox();

            CommonActions.ClickSave();
        }

        public void EditServiceRecipient(string associatedServiceName)
        {
            CommonActions.ClickLinkElement(ReviewSolutionsObjects.EditCatalogueItemServiceRecipientLink(GetAssociatedServiceID(associatedServiceName)));

            CommonActions.PageLoadedCorrectGetIndex(
             typeof(ServiceRecipientsController),
             nameof(ServiceRecipientsController.EditServiceRecipients)).Should().BeTrue();

            CommonActions.ClickFirstCheckbox();

            CommonActions.ClickCheckboxByLabel("BEECHWOOD MEDICAL CENTRE");

            CommonActions.ClickSave();
        }

        private string GetAssociatedServiceID(string associatedServiceName)
        {
            using var dbContext = Factory.DbContext;

            var associatedService = dbContext.AssociatedServices.FirstOrDefault(i => i.CatalogueItem.Name == associatedServiceName);

            return (associatedService != null) ? associatedService.CatalogueItemId.ToString() : string.Empty;
        }
    }
}
