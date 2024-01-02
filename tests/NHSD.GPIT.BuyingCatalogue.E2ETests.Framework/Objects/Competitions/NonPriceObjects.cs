using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Competitions
{
    public static class NonPriceObjects
    {
        public static By AddNonPriceElementLink => By.LinkText("Add a non-price element");

        public static By ElementRequirements => By.Id("Requirements");

        public static By AddAnotherRequirementLink => By.LinkText("Add another requirement");

        public static By FeatureWeighting => By.Id("Features");

        public static By ImplementationWeighting => By.Id("Implementation");

        public static By InteroperabilityWeightings => By.Id("Interoperability");

        public static By ServieLevelWeightings => By.Id("ServiceLevel");

        public static By TimeFrom => By.Id("TimeFrom");

        public static By TimeUntil => By.Id("TimeUntil");

        public static By EditCompareAndScoreLink(string elementtype) => By.XPath($"//a[contains(@href, 'scoring/{elementtype}')]");
    }
}
