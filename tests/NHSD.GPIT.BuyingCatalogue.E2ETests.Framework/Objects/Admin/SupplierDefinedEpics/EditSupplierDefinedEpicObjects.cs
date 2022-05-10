using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin.SupplierDefinedEpics
{
    public class EditSupplierDefinedEpicObjects : AddSupplierDefinedEpicObjects
    {
        public static By RelatedItemsTable => ByExtensions.DataTestId("epic-related-items-table");

        public static By RelatedItemsInset => ByExtensions.DataTestId("epic-related-items-inset");

        public static new By StatusInputError => By.Id("edit-supplier-defined-epic-error");

        public static By DeleteLink => By.LinkText("Delete Supplier defined Epic");
    }
}
