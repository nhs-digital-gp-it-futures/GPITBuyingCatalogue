using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin.SupplierDefinedEpics
{
    public class EditSupplierDefinedEpicDetailsObjects : AddSupplierDefinedEpicObjects
    {
        public static By RelatedItemsTable => ByExtensions.DataTestId("epic-related-items-table");

        public static By RelatedItemsInset => ByExtensions.DataTestId("epic-related-items-inset");

        public new static By StatusInputError => By.Id("edit-supplier-defined-epic-details-error");

        public static By DeleteLink => By.LinkText("Delete Supplier defined Epic");
    }
}
