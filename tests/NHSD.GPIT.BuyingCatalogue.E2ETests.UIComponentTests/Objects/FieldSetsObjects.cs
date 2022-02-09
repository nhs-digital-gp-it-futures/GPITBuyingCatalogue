using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Objects
{
    internal sealed class FieldSetsObjects
    {
        internal static By FieldSetsRadioListsLink => By.LinkText("Click here to view how they look on Radio Lists");
        internal static By FieldSetsYesNoRadiosLink => By.LinkText("Click here to view how they look on Yes No Radios");
        internal static By FieldSetsDateInputsLink => By.LinkText("Click here to view how they look on Date Inputs");
        internal static By FieldSetsCheckboxesLink => By.LinkText("Click here to view how they look on checkboxes");
    }
}
