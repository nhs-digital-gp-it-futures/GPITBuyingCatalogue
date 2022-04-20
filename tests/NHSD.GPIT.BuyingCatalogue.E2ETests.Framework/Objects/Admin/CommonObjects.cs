using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin
{
    public static class CommonObjects
    {
        public static By GoBackLink => By.ClassName("nhsuk-back-link__link");

        public static By SaveButton => By.Id("Submit");

        public static By ActionLink => By.ClassName("nhsuk-action-link__link");
    }
}
