using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
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

        public void EditServiceRecipient(string associatedServiceName)
        {
            CommonActions.PageLoadedCorrectGetIndex(
             typeof(ServiceRecipientsController),
             nameof(ServiceRecipientsController.SelectServiceRecipients)).Should().BeTrue();

            CommonActions.ClickFirstCheckbox();

            CommonActions.ClickCheckboxByLabel("BEECHWOOD MEDICAL CENTRE");

            CommonActions.ClickSave();
        }
    }
}
