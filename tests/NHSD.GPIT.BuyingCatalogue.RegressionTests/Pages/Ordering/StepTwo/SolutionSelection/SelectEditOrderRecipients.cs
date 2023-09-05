using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.SolutionSelection;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering.StepTwo.SolutionSelection
{
    public class SelectEditOrderRecipients : PageBase
    {
        public SelectEditOrderRecipients(IWebDriver driver, CommonActions commonActions, LocalWebApplicationFactory factory)
            : base(driver, commonActions)
        {
            Factory = factory;
        }

        public LocalWebApplicationFactory Factory { get; }

        public void AddCatalogueSolutionServiceRecipient(int multipleServiceRecipients, bool allServiceRecipients)
        {
            CommonActions.PageLoadedCorrectGetIndex(
              typeof(ServiceRecipientsController),
              nameof(ServiceRecipientsController.SelectServiceRecipients)).Should().BeTrue();

            CommonActions.ClickFirstExpander();

            if (multipleServiceRecipients > 0 && !allServiceRecipients)
                CommonActions.ClickMultipleCheckboxes(multipleServiceRecipients);
            else if (multipleServiceRecipients == 0 && allServiceRecipients)
                CommonActions.ClickAllCheckboxes();
            else
                CommonActions.ClickFirstCheckbox();

            CommonActions.ClickSave();
        }
    }
}
