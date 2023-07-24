using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Capabilities;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.SupplierDefinedEpics;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.SupplierDefinedEpics
{
    public sealed class AddSupplierDefinedEpicDetailsValidator : AbstractValidator<AddSupplierDefinedEpicDetailsModel>
    {
        private readonly ISupplierDefinedEpicsService supplierDefinedEpicsService;

        public AddSupplierDefinedEpicDetailsValidator(
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
                .When(m => m.SelectedCapabilityIds?.Length > 0 && m.IsActive.HasValue)
                .OverridePropertyName(
                    m => m.SelectedCapabilityIds,
                    m => m.Name,
                    m => m.Description,
                    m => m.IsActive);

            RuleFor(m => m.SelectedCapabilityIds)
                .NotNull()
                .WithMessage("Select a Capability");
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
