using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.SupplierDefinedEpics;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin.SupplierDefinedEpics
{
    public class AddSupplierDefinedEpicObjects
    {
        public static By NameInput => By.Id(nameof(AddSupplierDefinedEpicDetailsModel.Name));

        public static By DescriptionInput => By.Id(nameof(AddSupplierDefinedEpicDetailsModel.Description));

        public static By StatusInput => ByExtensions.DataTestId("active-status-buttons");

        public static By NameInputError => By.Id($"{nameof(AddSupplierDefinedEpicDetailsModel.Name)}-error");

        public static By DescriptionInputError => By.Id($"{nameof(AddSupplierDefinedEpicDetailsModel.Description)}-error");

        public static By StatusInputError => By.Id($"supplier-defined-epic-base-error");
    }
}
