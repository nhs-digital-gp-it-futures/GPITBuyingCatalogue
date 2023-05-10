using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering.Contracts
{
    public static class ImplementationPlanObjects
    {
        public static By UseDefaultMilestonesError => By.Id("default-implementation-plan-error");

        public static By ImplementationPlanMilestonesLink => By.LinkText("Implementation plan milestones");

        public static By ImplementationMilestonesAndPaymentTriggers => By.LinkText("Implementation milestones and payment triggers");

        public static string BespokeMilestonesAgreed => "No, I've agreed bespoke milestones with the supplier";
    }
}
