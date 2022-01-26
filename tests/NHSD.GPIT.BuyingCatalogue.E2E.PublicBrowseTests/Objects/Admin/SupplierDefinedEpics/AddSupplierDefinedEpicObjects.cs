using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.SupplierDefinedEpics;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Admin.SupplierDefinedEpics
{
    internal static class AddSupplierDefinedEpicObjects
    {
        internal static By CapabilityInput => By.Id(nameof(AddEditSupplierDefinedEpicModel.SelectedCapabilityId));

        internal static By NameInput => By.Id(nameof(AddEditSupplierDefinedEpicModel.Name));

        internal static By DescriptionInput => By.Id(nameof(AddEditSupplierDefinedEpicModel.Description));

        internal static By StatusInput => ByExtensions.DataTestId("active-status-buttons");

        internal static By CapabilityInputError => By.Id($"{nameof(AddEditSupplierDefinedEpicModel.SelectedCapabilityId)}-error");

        internal static By NameInputError => By.Id($"{nameof(AddEditSupplierDefinedEpicModel.Name)}-error");

        internal static By DescriptionInputError => By.Id($"{nameof(AddEditSupplierDefinedEpicModel.Description)}-error");

        internal static By StatusInputError => By.Id($"add-edit-supplier-defined-epic-error");
    }
}
