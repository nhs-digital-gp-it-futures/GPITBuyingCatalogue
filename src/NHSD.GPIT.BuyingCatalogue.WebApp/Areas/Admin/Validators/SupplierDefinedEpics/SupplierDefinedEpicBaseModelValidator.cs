using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Capabilities;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.SupplierDefinedEpics;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.SupplierDefinedEpics
{
    public sealed class SupplierDefinedEpicBaseModelValidator : AbstractValidator<SupplierDefinedEpicBaseModel>
    {
        public const string CapabilityError = "Select a Capability";
        public const string EpicNameError = "Enter an Epic name";
        public const string DescriptionError = "Enter a description";
        public const string StatusError = "Select a status";
        public const string SupplierDefinedEpicAlreadyExists = "A supplier defined Epic with these details already exists";

        private readonly ISupplierDefinedEpicsService supplierDefinedEpicsService;

        public SupplierDefinedEpicBaseModelValidator(
            ISupplierDefinedEpicsService supplierDefinedEpicsService)
        {
            this.supplierDefinedEpicsService = supplierDefinedEpicsService;

            RuleFor(m => m.SelectedCapabilityId)
                .NotNull()
                .WithMessage(CapabilityError);

            RuleFor(m => m.Name)
                .NotEmpty()
                .WithMessage(EpicNameError);

            RuleFor(m => m.Description)
                .NotEmpty()
                .WithMessage(DescriptionError);

            RuleFor(m => m.IsActive)
                .NotNull()
                .WithMessage(StatusError);

            RuleFor(m => m)
                .Must(NotBeADuplicateEpic)
                .WithMessage(SupplierDefinedEpicAlreadyExists)
                .When(m => m.SelectedCapabilityId.HasValue && m.IsActive.HasValue)
                .OverridePropertyName(
                    m => m.SelectedCapabilityId,
                    m => m.Name,
                    m => m.Description,
                    m => m.IsActive);
        }

        private bool NotBeADuplicateEpic(SupplierDefinedEpicBaseModel model)
        {
            return !supplierDefinedEpicsService.EpicExists(
                model.Id,
                model.SelectedCapabilityId!.Value,
                model.Name,
                model.Description,
                model.IsActive!.Value).GetAwaiter().GetResult();
        }
    }
}
