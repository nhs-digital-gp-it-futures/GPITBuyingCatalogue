using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin.ManageOrders
{
    public static class DeleteOrderObjects
    {
        public static By WarningCallout => ByExtensions.DataTestId("delete-warning-callout");

        public static By NameOfRequester => By.Id("NameOfRequester");
        public static By NameOfApprover => By.Id("NameOfApprover");
        public static By DateOfApproval => By.Id("delete-approve-date");

        public static By DateOfApprovalDayInput => By.Id("ApprovalDay");

        public static By DateOfApprovalMonthInput => By.Id("ApprovalMonth");

        public static By DateOfApprovalYearInput => By.Id("ApprovalYear");

        public static By DeleteRequestNameError => By.PartialLinkText("Enter who requested the deletion");
        public static By DeleteApproveNameError => By.PartialLinkText("Enter who approved the deletion");
        public static By DeleteDayMissingError => By.PartialLinkText(DeleteOrderModelValidator.ApprovalDateDayMissingErrorMessage);
        public static By DeleteMonthMissingError => By.PartialLinkText(DeleteOrderModelValidator.ApprovalDateMonthMissingErrorMessage);
        public static By DeleteYearMissingError => By.PartialLinkText(DeleteOrderModelValidator.ApprovalDateYearMissingErrorMessage);
        public static By DeleteYearTooShortError => By.PartialLinkText(DeleteOrderModelValidator.ApprovalDateYearTooShortErrorMessage);
        public static By DeleteInvalidDateError => By.PartialLinkText(DeleteOrderModelValidator.ApprovalDateInvalidErrorMessage);
        public static By DeleteDateInFutureError => By.PartialLinkText(DeleteOrderModelValidator.ApprovalDateInTheFutureErrorMessage);
    }
}
