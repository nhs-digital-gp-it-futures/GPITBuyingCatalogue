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

            RuleFor(m => m.Name)
                .NotEmpty()
                .WithMessage("Enter a name");

            RuleFor(m => m.Description)
                .NotEmpty()
                .WithMessage("Enter a description");

            RuleFor(m => m.IsActive)
                .NotNull()
                .WithMessage("Select a status");

            RuleFor(m => m)
                .Must(NotBeADuplicateEpic)
                .WithMessage("An Epic with this name already exists. Try another name")
                .When(m => m.IsActive.HasValue)
                .OverridePropertyName(
                    m => m.Name);

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
            return !supplierDefinedEpicsService.EpicWithNameExists(
                model.Id,
                model.Name).GetAwaiter().GetResult();
        }
    }
}
