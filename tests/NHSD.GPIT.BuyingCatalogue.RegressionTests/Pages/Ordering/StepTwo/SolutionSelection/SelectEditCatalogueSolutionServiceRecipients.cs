using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering.SolutionSelection;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.SolutionSelection;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering.StepTwo.SolutionSelection
{
    public class SelectEditCatalogueSolutionServiceRecipients : PageBase
    {
        public SelectEditCatalogueSolutionServiceRecipients(IWebDriver driver, CommonActions commonActions, LocalWebApplicationFactory factory)
            : base(driver, commonActions)
        {
            Factory = factory;
        }

        public LocalWebApplicationFactory Factory { get; }

        public void AddCatalogueSolutionServiceRecipient(int multipleServiceRecipients, bool allServiceRecipients)
        {
            CommonActions.PageLoadedCorrectGetIndex(
              typeof(ServiceRecipientsController),
              nameof(ServiceRecipientsController.AddServiceRecipients)).Should().BeTrue();

            if (multipleServiceRecipients > 0 && !allServiceRecipients)
                CommonActions.ClickMultipleCheckboxes(multipleServiceRecipients);
            else if (multipleServiceRecipients == 0 && allServiceRecipients)
                CommonActions.ClickAllCheckboxes();
            else
                CommonActions.ClickFirstCheckbox();

            CommonActions.ClickSave();
        }

        public void EditCatalogueSolutionServiceRecipient(string solutionName)
        {
            CommonActions.ClickLinkElement(ReviewSolutionsObjects.EditCatalogueItemServiceRecipientLink(GetCatalogueSolutionID(solutionName)));

            CommonActions.PageLoadedCorrectGetIndex(
              typeof(ServiceRecipientsController),
              nameof(ServiceRecipientsController.EditServiceRecipients)).Should().BeTrue();

            CommonActions.ClickFirstCheckbox();

            CommonActions.ClickCheckboxByLabel("BEECHWOOD MEDICAL CENTRE");

            CommonActions.ClickSave();
        }

        public void AmendEditCatalogueSolutionServiceRecipient(string solutionName, int multipleServiceRecipients)
        {
            CommonActions.ClickLinkElement(ReviewSolutionsObjects.EditCatalogueItemServiceRecipientLink(GetCatalogueSolutionID(solutionName)));

            CommonActions.PageLoadedCorrectGetIndex(
            typeof(ServiceRecipientsController),
            nameof(ServiceRecipientsController.EditServiceRecipients)).Should().BeTrue();

            if (multipleServiceRecipients == 0)
            {
                CommonActions.ClickFirstCheckbox();
            }
            else
            {
                CommonActions.ClickMultipleCheckboxes(multipleServiceRecipients);
            }

            CommonActions.ClickSave();
        }

        public void AmendImportServiceRecipients(string solutionName, string fileName)
        {
            CommonActions.ClickLinkElement(ReviewSolutionsObjects.EditCatalogueItemServiceRecipientLink(GetCatalogueSolutionID(solutionName)));

            CommonActions.PageLoadedCorrectGetIndex(
            typeof(ServiceRecipientsController),
            nameof(ServiceRecipientsController.EditServiceRecipients)).Should().BeTrue();

            CommonActions.ClickLinkElement(ServiceRecipientObjects.ImportServiceRecipients);

            var importFile = CommonActions.GetRecipientImportCsv(fileName);

            CommonActions.UploadFile(ServiceRecipientObjects.ImportRecipientsFileInput, importFile);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
            typeof(ServiceRecipientsController),
            nameof(ServiceRecipientsController.EditServiceRecipients)).Should().BeTrue();

            CommonActions.ClickSave();
        }

        private string GetCatalogueSolutionID(string solutionName)
        {
            using var dbContext = Factory.DbContext;

            var solution = dbContext.Solutions.FirstOrDefault(i => i.CatalogueItem.Name == solutionName);

            return (solution != null) ? solution.CatalogueItemId.ToString() : string.Empty;
        }
    }
}
