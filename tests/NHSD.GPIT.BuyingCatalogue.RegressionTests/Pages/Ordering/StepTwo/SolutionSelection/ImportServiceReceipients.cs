using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
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
            UploadServiceRecipients();
            var importFile = CommonActions.ImportCsvFile(fileName);

            CommonActions.UploadFile(ServiceRecipientObjects.ImportRecipientsFileInput, importFile);

            CommonActions.ClickSave();
        }

        public void UploadServiceRecipients()
        {
            CommonActions.ClickLinkElement(OrderRecipientsObjects.ServiceRecipientsLink);
            CommonActions.LedeText().Should().Be("Select how you want to add Service Recipients.".FormatForComparison());
            CommonActions.ClickRadioButtonWithText("Upload Service Recipients using a CSV file");
            CommonActions.ClickSave();

            CommonActions.LedeText().Should().Be("Create a CSV with your Service Recipients in the first column and their ODS codes in the second column.".FormatForComparison());
        }
    }
}
