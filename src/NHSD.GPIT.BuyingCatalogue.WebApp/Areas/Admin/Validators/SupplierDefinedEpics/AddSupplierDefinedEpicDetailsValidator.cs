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
                .WithMessage("An Epic with this name already exists. Try another name")
                .OverridePropertyName(
                    m => m.Id,
                    m => m.Name);

            RuleFor(m => m.SelectedCapabilityIds)
                .NotNull()
                .WithMessage("Select a Capability");
        }

        private bool NotBeADuplicateEpic(AddSupplierDefinedEpicDetailsModel model)
        {
            return !supplierDefinedEpicsService.EpicWithNameExists(
                model.Id,
                model.Name).GetAwaiter().GetResult();
        }
    }
}
