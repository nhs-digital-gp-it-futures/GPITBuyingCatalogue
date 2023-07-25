using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Capabilities;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.SupplierDefinedEpics;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.SupplierDefinedEpics
{
    public sealed class EditSupplierDefinedEpicModelValidator : AbstractValidator<EditSupplierDefinedEpicDetailsModel>
    {
        private readonly ISupplierDefinedEpicsService supplierDefinedEpicsService;

        public EditSupplierDefinedEpicModelValidator(
            ISupplierDefinedEpicsService supplierDefinedEpicsService)
        {
            this.supplierDefinedEpicsService = supplierDefinedEpicsService;

            RuleFor(m => m.Name)
                .NotEmpty()
                .WithMessage("Enter an Epic name");

            RuleFor(m => m.Description)
                .NotEmpty()
                .WithMessage("Enter a description");

            RuleFor(m => m.IsActive)
                .NotNull()
                .WithMessage("Select a status");

            RuleFor(m => m)
                .Must(NotBeADuplicateEpic)
                .WithMessage("A supplier defined Epic with these details already exists")
                .When(m => m.IsActive.HasValue)
                .OverridePropertyName(
                    m => m.Id,
                    m => m.Name,
                    m => m.Description,
                    m => m.IsActive);

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

        private bool NotBeADuplicateEpic(AddSupplierDefinedEpicDetailsModel model)
        {
            return !supplierDefinedEpicsService.EpicExists(
                model.Id,
                model.Name,
                model.Description,
                model.IsActive!.Value).GetAwaiter().GetResult();
        }
    }
}
