using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering.SolutionSelection;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.SolutionSelection;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering.StepTwo.SolutionSelection
{
    public class SelectEditAdditionalServiceRecipients : PageBase
    {
        public SelectEditAdditionalServiceRecipients(IWebDriver driver, CommonActions commonActions, LocalWebApplicationFactory factory)
            : base(driver, commonActions)
        {
            Factory = factory;
        }

        public LocalWebApplicationFactory Factory { get; }

        public void AddServiceRecipients()
        {
            CommonActions.PageLoadedCorrectGetIndex(
             typeof(ServiceRecipientsController),
             nameof(ServiceRecipientsController.AddServiceRecipients)).Should().BeTrue();

            CommonActions.ClickSave();
        }

        public void AmendEditAdditionalServiceRecipient(string additionalService)
        {
            CommonActions.ClickLinkElement(ReviewSolutionsObjects.EditCatalogueItemServiceRecipientLink(GetAdditionalServiceID(additionalService)));

            CommonActions.PageLoadedCorrectGetIndex(
            typeof(ServiceRecipientsController),
            nameof(ServiceRecipientsController.EditServiceRecipients)).Should().BeTrue();

            CommonActions.ClickCheckboxByLabel("BRIG ROYD SURGERY");

            CommonActions.ClickSave();
        }

        public void EditAdditionalServiceRecipient(string additionalServiceName)
        {
            CommonActions.ClickLinkElement(ReviewSolutionsObjects.EditCatalogueItemServiceRecipientLink(GetAdditionalServiceID(additionalServiceName)));

            CommonActions.PageLoadedCorrectGetIndex(
              typeof(ServiceRecipientsController),
              nameof(ServiceRecipientsController.EditServiceRecipients)).Should().BeTrue();

            CommonActions.ClickFirstCheckbox();

            CommonActions.ClickCheckboxByLabel("BEECHWOOD MEDICAL CENTRE");

            CommonActions.ClickSave();
        }

        private string GetAdditionalServiceID(string additionalServiceName)
        {
            using var db = Factory.DbContext;

            var additionalService = db.AdditionalServices.FirstOrDefault(a => a.CatalogueItem.Name == additionalServiceName);

            return (additionalService != null) ? additionalService.CatalogueItemId.ToString() : string.Empty;
        }
    }
}
