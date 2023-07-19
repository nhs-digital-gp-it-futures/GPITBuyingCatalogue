using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Capabilities;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.SupplierDefinedEpics;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.SupplierDefinedEpics
{
    public sealed class EditSupplierDefinedEpicModelValidator : AbstractValidator<EditSupplierDefinedEpicDetailsModel>
    {
        private readonly ISupplierDefinedEpicsService supplierDefinedEpicsService;

        public EditSupplierDefinedEpicModelValidator(
            ISupplierDefinedEpicsService supplierDefinedEpicsService)
        {
            this.supplierDefinedEpicsService = supplierDefinedEpicsService;

            Include(new EpicDetailsValidator(supplierDefinedEpicsService));

            RuleFor(m => m.IsActive)
                .Must(NotBeReferencedByAnySolutions)
                .WithMessage("This supplier defined Epic cannot be set to inactive as it is referenced by another solution or service")
                .When(m => m.IsActive == false);
        }

        public bool NotBeReferencedByAnySolutions(EditSupplierDefinedEpicDetailsModel model, bool? isActive)
        {
            var epic = supplierDefinedEpicsService.GetEpic(model.Id).GetAwaiter().GetResult();
            if (epic.IsActive == isActive)
                return true;

            var itemsReferencingEpic = supplierDefinedEpicsService.GetItemsReferencingEpic(model.Id).GetAwaiter().GetResult();
            return itemsReferencingEpic.Count == 0;
        }
    }
}
