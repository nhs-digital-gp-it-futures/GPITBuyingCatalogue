using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Admin.AssociatedServices
{
    public static class AssociatedServices
    {
        public static By EditLink => ByExtensions.DataTestId("edit-link");

        public static By AddAssociatedServiceContinueButton => By.LinkText("Continue");
    }
}
