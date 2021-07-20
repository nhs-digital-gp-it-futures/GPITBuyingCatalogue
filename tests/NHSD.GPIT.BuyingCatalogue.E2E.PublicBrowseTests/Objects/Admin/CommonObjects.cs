using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Admin
{
    internal static class CommonObjects
    {
        internal static By GoBackLink => By.ClassName("nhsuk-back-link__link");

        internal static By SaveButton => By.Id("Submit");
    }
}
