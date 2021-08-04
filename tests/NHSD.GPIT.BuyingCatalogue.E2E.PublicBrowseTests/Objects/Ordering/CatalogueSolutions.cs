using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Ordering
{
    public static class CatalogueSolutions
    {
        public static By SelectCatalogueSolutionErrorMessage => By.Id("select-solution-error");

        public static By SelectCatalogueSolutionPriceErrorMessage => By.Id("select-solution-price-error");

        public static By CatalogueSolutionsRecipientsSelectAllButton => By.ClassName("nhsuk-button--secondary");

        public static By CatalogueSolutionsRecipientsTable => By.ClassName("nhsuk-table-responsive");

        public static By CatalogueSolutionsRecipientsErrorMessage => By.ClassName("nhsuk-error-message");
    }
}
