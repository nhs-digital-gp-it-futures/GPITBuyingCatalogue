using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Admin.SupplierDefinedEpics
{
    internal class EditSupplierDefinedEpicObjects : AddSupplierDefinedEpicObjects
    {
        internal static By RelatedItemsTable => ByExtensions.DataTestId("epic-related-items-table");

        internal static By RelatedItemsInset => ByExtensions.DataTestId("epic-related-items-inset");

        internal static new By StatusInputError => By.Id("edit-supplier-defined-epic-error");

        internal static By DeleteLink => By.LinkText("Delete Supplier defined Epic");
    }
}
