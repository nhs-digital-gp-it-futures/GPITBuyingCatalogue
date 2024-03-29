﻿using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering.Contracts
{
    public static class ImplementationPlanObjects
    {
        public static By UseDefaultMilestonesError => By.Id("default-implementation-plan-error");

        public static By ImplementationPlanMilestonesLink => By.LinkText("Implementation plan milestones");

        public static By ImplementationMilestonesAndPaymentTriggers => By.LinkText("Implementation milestones and payment triggers");

        public static By ImplementationMilestonesAndAccociatedService => By.LinkText("Associated Service milestones and requirements");

        public static string BespokeMilestonesAgreed => "No, I've agreed bespoke milestones with the supplier";

        public static By ImplementationPlanAddBespokeMilestone => By.LinkText("Add a bespoke milestone");

        public static By AssociatedServicesAddBespokeMilestone => By.LinkText("Add a milestone");

        public static By MileStoneName => By.ClassName("nhsuk-input");

        public static string MileStoneValue => "Milestone";

        public static By MilestonePaymentTrigger => By.Id("PaymentTrigger");

        public static By MilestoneAssociatedServiceUnits => By.Id("Quantity");
    }
}
