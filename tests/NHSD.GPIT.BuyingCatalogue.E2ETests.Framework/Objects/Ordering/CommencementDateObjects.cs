using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering
{
    public static class CommencementDateObjects
    {
        public static By CancelLink => By.LinkText("Cancel");

        public static By CommencementDateReadOnlyDisplay => By.Id("commencement-date-read-only-display");

        public static By CommencementDateDayInput => By.Id("Day");

        public static By CommencementDateMonthInput => By.Id("Month");

        public static By CommencementDateYearInput => By.Id("Year");

        public static By CommencementDateErrorMessage => By.Id("commencement-date-error");

        public static By ConfirmChanges => By.Id("confirm-changes");

        public static By ConfirmChangesError => By.Id("confirm-changes-error");

        public static By InitialPeriodInput => By.Id("InitialPeriod");

        public static By InitialPeriodError => By.Id("InitialPeriod-error");

        public static By MaximumTermInput => By.Id("MaximumTerm");

        public static By MaximumTermError => By.Id("MaximumTerm-error");

        public static By TimescalesForCallOffLink => By.LinkText("Timescales for Call-off Agreement");
    }
}
