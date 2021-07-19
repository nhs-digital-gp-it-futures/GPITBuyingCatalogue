using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Ordering
{
    internal static class CommencementDate
    {
        public static By CommencementDateDayInput => By.Id("Day");

        public static By CommencementDateMonthInput => By.Id("Month");

        public static By CommencementDateYearInput => By.Id("Year");

        public static By CommencementDateErrorMessage => By.Id("commencement-date-error");
    }
}
