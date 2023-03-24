using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering
{
    public static class FundingSources
    {
        public static By SelectFundingSourcesLink => By.LinkText("Select funding sources");

        public static By EditableFundingSourcesTable => ByExtensions.DataTestId("funding-sources-items-editable");

        public static By LocalOnlyFundingSourcesTable => ByExtensions.DataTestId("funding-sources-items-local-only");

        public static By ChangeFramework => ByExtensions.DataTestId("funding-sources-multiple-frameworks-change");

        public static By NoFundingRequiredSourcesTable => ByExtensions.DataTestId("funding-sources-items-no-funding-required");

        public static By EditLink => By.LinkText("Edit");

        public static By FundingSource => By.Id("funding-source");

        public static By FundingSourceError => By.Id("funding-source-error");

        public static By SelectFramework => By.Id("selected-framework");

        public static By SelectFrameworkError => By.Id("selected-framework-error");

        public static By ConfirmChangeError => By.Id("confirm-changes-error");
    }
}
