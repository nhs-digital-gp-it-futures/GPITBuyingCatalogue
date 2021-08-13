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

        public static By CatalogueSolutionsRecipientsDate => By.ClassName("nhsuk-date-input__input");

        public static By CatalogueSolutionsRecipientsDateErrorMessage => By.Id("select-solution-service-recipients-date-error");

        public static By CatalogueSolutionsSelectFlatDeclarativeAndOnDemandQuantityInput => By.Id("Quantity");

        public static By CatalogueSolutionsSelectFlatDeclarativeAndOnDemandQuantityInputErrorMessage => By.Id("Quantity-error");

        public static By CatalogueSolutionsSelectFlatOnDemandRadioInputErrorMessage => By.Id("select-flat-on-demand-quantity-error");

        public static By CatalogueSolutionsEditSolutionAgreedPriceInput => By.Id("OrderItem_AgreedPrice");

        public static By CatalogueSolutionsEditSolutionAgreedPriceInputErrorMessage => By.Id("OrderItem_AgreedPrice-error");

        public static By CatalogueSolutionsEditSolutionFirstQuantityInput => By.Id("OrderItem_ServiceRecipients_0__Quantity");

        public static By CatalogueSolutionsEditSolutionFirstQuantityInputErrorMessage => By.Id("OrderItem_ServiceRecipients_0__Quantity-error");

        public static By CatalogueSolutionsEditSolutionFirstDateDayInput => By.Id("OrderItem_ServiceRecipients_0__Day");

        public static By CatalogueSOlutionsEditSolutionFirstDateInputErrorMessage => By.Id("edit-solution-error");

        public static By CatalogueSolutionsEditSolutionFirstDateMonthInput => By.Id("OrderItem_ServiceRecipients_0__Month");

        public static By CatalogueSolutionsEditSolutionFirstDateYearInput => By.Id("OrderItem_ServiceRecipients_0__Year");

        public static By CatalogueSolutionsEditSolutionEditServiceRecipientsButton => By.ClassName("nhsuk-button--secondary");

        public static By CatalogueSolutionsEditSolutionDeleteSolutionLink => By.LinkText("Delete Catalogue Solution");
    }
}
