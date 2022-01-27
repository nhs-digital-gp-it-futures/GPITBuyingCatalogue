using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.SupplierDefinedEpics;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Admin.SupplierDefinedEpics
{
    internal class AddSupplierDefinedEpicObjects
    {
        internal static By CapabilityInput => By.Id(nameof(SupplierDefinedEpicBaseModel.SelectedCapabilityId));

        internal static By NameInput => By.Id(nameof(SupplierDefinedEpicBaseModel.Name));

        internal static By DescriptionInput => By.Id(nameof(SupplierDefinedEpicBaseModel.Description));

        internal static By StatusInput => ByExtensions.DataTestId("active-status-buttons");

        internal static By CapabilityInputError => By.Id($"{nameof(SupplierDefinedEpicBaseModel.SelectedCapabilityId)}-error");

        internal static By NameInputError => By.Id($"{nameof(SupplierDefinedEpicBaseModel.Name)}-error");

        internal static By DescriptionInputError => By.Id($"{nameof(SupplierDefinedEpicBaseModel.Description)}-error");

        internal static By StatusInputError => By.Id($"supplier-defined-epic-base-error");
    }
}
