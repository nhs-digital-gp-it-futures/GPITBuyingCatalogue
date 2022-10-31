using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering.SolutionSelection;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers.SolutionSelection;
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

        public void AddCatalogueSolutionServiceRecipient(bool multipleServiceRecipients)
        {
            CommonActions.PageLoadedCorrectGetIndex(
              typeof(ServiceRecipientsController),
              nameof(ServiceRecipientsController.AddServiceRecipients)).Should().BeTrue();

            if (multipleServiceRecipients)
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

            CommonActions.ClickCheckboxByLabel("BEVAN LIMITED");

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
             typeof(TaskListController),
             nameof(TaskListController.TaskList)).Should().BeTrue();
        }

        private string GetCatalogueSolutionID(string solutionName)
        {
            using var dbContext = Factory.DbContext;

            var solution = dbContext.Solutions.SingleOrDefault(i => i.CatalogueItem.Name == solutionName);

            return (solution != null) ? solution.CatalogueItemId.ToString() : string.Empty;
        }
    }
}
