﻿using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Ordering
{
    internal static class FundingSources
    {
        public static By EditableFundingSourcesTable => ByExtensions.DataTestId("funding-sources-items-editable");

        public static By LocalOnlyFundingSourcesTable => ByExtensions.DataTestId("funding-sources-items-local-only");

        public static By EditLink => By.LinkText("Edit");

        public static By FundingSource => By.Id("funding-source");

        public static By FundingSourceError => By.Id("funding-source-error");

        public static By AmountOfCentralFunding => By.Id("AmountOfCentralFunding");

        public static By AmountOfCentralFundingError => By.Id("AmountOfCentralFunding-error");
    }
}
