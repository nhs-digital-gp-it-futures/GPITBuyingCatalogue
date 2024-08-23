using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering.Contracts
{
    public static class DeliveryDatesObjects
    {
        public static By AmendDateError => By.Id("amend-date-error");

        public static By ConfirmChanges => By.Id("confirm-changes");

        public static By ConfirmChangesError => By.Id("confirm-changes-error");

        public static By EditDatesEditDeliveryDateLink => By.Id("edit-delivery-date-link");

        public static By EditDatesDateError => By.Id("edit-dates-error");

        public static By EditDatesDescription(int index) => By.Id($"Recipients_{index}__Description");

        public static By EditDatesDayInput(int index) => By.Id($"Recipients_{index}__Value_{index}__Day");

        public static By EditDatesMonthInput(int index) => By.Id($"Recipients_{index}__Value_{index}__Month");

        public static By EditDatesYearInput(int index) => By.Id($"Recipients_{index}__Value_{index}__Year");

        public static By MatchDates => By.Id("match-dates");

        public static By MatchDatesError => By.Id("match-dates-error");

        public static By ReviewChangeDeliveryDateLink => By.Id("change-delivery-date-link");

        public static By ReviewEditDeliveryDatesLink(string itemName) => By.LinkText($"Edit planned delivery dates for {itemName}");

        public static By SelectDateDayInput => By.Id("Day");

        public static By SelectDateMonthInput => By.Id("Month");

        public static By SelectDateYearInput => By.Id("Year");

        public static By SelectDateError => By.Id("select-date-error");
    }
}
