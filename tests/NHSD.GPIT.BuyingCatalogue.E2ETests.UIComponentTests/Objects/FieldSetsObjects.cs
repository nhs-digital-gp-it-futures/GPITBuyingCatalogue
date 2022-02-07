using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Objects
{
    internal sealed class FieldSetsObjects
    {
        internal static By FieldSets => By.CssSelector("#maincontent>div>div>div>div.app-pane__main-content>h1");
        internal static By FieldSetsCheckboxesLink => By.XPath("//span[text()='Click here to view how they look on checkboxes']");
        internal static By FieldSetsRadioListsLink => By.XPath("//span[text()='Click here to view how they look on Radio Lists']");
        internal static By FieldSetsYesNoRadiosLink => By.XPath("//span[text()='Click here to view how they look on Yes No Radios']");
        internal static By FieldSetsDateInputsLink => By.XPath("//span[text()='Click here to view how they look on Date Inputs']");
    }
}
