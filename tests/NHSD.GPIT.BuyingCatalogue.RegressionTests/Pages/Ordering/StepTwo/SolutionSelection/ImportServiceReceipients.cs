using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.SolutionSelection;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering.StepTwo.SolutionSelection
{
    internal class ImportServiceReceipients : PageBase
    {
        public ImportServiceReceipients(IWebDriver driver, CommonActions commonActions)
            : base(driver, commonActions)
        {
        }

        public void ImportServiceRecipients(string fileName)
        {
            CommonActions.PageLoadedCorrectGetIndex(
              typeof(ServiceRecipientsController),
              nameof(ServiceRecipientsController.SelectServiceRecipients)).Should().BeTrue();

            CommonActions.ClickLinkElement(ServiceRecipientObjects.ImportServiceRecipients);

            var importFile = CommonActions.GetRecipientImportCsv(fileName);

            CommonActions.UploadFile(ServiceRecipientObjects.ImportRecipientsFileInput, importFile);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
              typeof(ServiceRecipientsController),
              nameof(ServiceRecipientsController.SelectServiceRecipients)).Should().BeTrue();

            CommonActions.ClickSave();
        }
    }
}
