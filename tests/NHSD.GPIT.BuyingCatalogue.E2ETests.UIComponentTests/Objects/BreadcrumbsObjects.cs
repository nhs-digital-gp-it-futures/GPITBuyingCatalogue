using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Objects
{
    internal sealed class BreadcrumbsObjects
    {
        internal static By BreadcrumbsActionLink => (By.LinkText("Action Links"));

        internal static By BreadcrumbsAddressLink => (By.LinkText("Address"));
    }
}
