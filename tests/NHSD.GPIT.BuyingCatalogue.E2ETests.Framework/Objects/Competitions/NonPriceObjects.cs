using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Competitions
{
    public static class NonPriceObjects
    {
        public static By AddFeaturesLink => ByExtensions.DataTestId("features-link");

        public static By AddImplementationLink => ByExtensions.DataTestId("implementation-link");

        public static By AddInteroperabilityLink => ByExtensions.DataTestId("interoperability-link");

        public static By AddServiceLevelsLink => ByExtensions.DataTestId("service-levels-link");

        public static By ElementRequirements => By.Id("Requirements");

        public static By AddAnotherRequirementLink => By.LinkText("Add another requirement");

        public static By FeatureWeighting => By.Id("Features");

        public static By ImplementationWeighting => By.Id("Implementation");

        public static By InteroperabilityWeightings => By.Id("Interoperability");

        public static By ServiceLevelWeightings => By.Id("ServiceLevel");

        public static By TimeFrom => By.Id("TimeFrom");

        public static By TimeUntil => By.Id("TimeUntil");

        public static By EditCompareAndScoreLink(string elementtype) => By.XPath($"//a[contains(@href, 'scoring/{elementtype}')]");
    }
}
