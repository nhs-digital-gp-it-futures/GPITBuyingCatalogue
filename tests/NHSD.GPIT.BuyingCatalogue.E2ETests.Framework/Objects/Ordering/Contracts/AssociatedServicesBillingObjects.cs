using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering.Contracts
{
    public static class AssociatedServicesBillingObjects
    {
        public static By UseDefaultBillingError => By.Id("review-billing-error");

        public static By HasSpecificRequirementsError => By.Id("specific-requirements-error");
    }
}
