using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering.Contracts
{
    public static class AssociatedServicesBillingObjects
    {
        public static By UseDefaultBillingError => By.Id("review-billing-error");

        public static By HasSpecificRequirementsError => By.Id("specific-requirements-error");

        public static By AssociatedServiceBillingAndRequirementsLink => By.LinkText("Associated Service milestones and requirements");

        public static string BespokeMilestonesAgreed => "No, I've agreed bespoke milestones with the supplier";

        public static string SpecificRequirementsAgreed => "No, I’ve agreed some specific requirements with the supplier";

    }
}
