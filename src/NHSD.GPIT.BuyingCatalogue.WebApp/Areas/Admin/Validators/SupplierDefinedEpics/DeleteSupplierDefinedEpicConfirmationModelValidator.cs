using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Capabilities;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.SupplierDefinedEpics;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.SupplierDefinedEpics
{
    public sealed class DeleteSupplierDefinedEpicConfirmationModelValidator
        : AbstractValidator<DeleteSupplierDefinedEpicConfirmationModel>
    {
        private readonly ISupplierDefinedEpicsService supplierDefinedEpicsService;

        public DeleteSupplierDefinedEpicConfirmationModelValidator(
            ISupplierDefinedEpicsService supplierDefinedEpicsService)
        {
            this.supplierDefinedEpicsService = supplierDefinedEpicsService;

            RuleFor(m => m)
                .Must(NotBeActiveOrReferenced);
        }

        private bool NotBeActiveOrReferenced(DeleteSupplierDefinedEpicConfirmationModel model)
        {
            var epic = supplierDefinedEpicsService.GetEpic(model.Id).GetAwaiter().GetResult();
            var referencingItems = supplierDefinedEpicsService.GetItemsReferencingEpic(model.Id).GetAwaiter().GetResult();

            return !epic.IsActive && referencingItems.Count == 0;
        }
    }
}
