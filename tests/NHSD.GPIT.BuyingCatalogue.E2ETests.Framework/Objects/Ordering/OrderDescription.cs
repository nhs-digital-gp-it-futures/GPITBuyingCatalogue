using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering
{
    public static class OrderDescription
    {
        public static By DescriptionInput => By.Id("Description");

        public static By DescriptionInputError => By.Id("Description-error");
    }
}
