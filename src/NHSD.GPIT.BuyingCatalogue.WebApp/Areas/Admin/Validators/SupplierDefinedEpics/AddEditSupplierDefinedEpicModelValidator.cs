using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Capabilities;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.SupplierDefinedEpics;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.SupplierDefinedEpics
{
    public sealed class AddEditSupplierDefinedEpicModelValidator : AbstractValidator<AddEditSupplierDefinedEpicModel>
    {
        private readonly ISupplierDefinedEpicsService supplierDefinedEpicsService;

        public AddEditSupplierDefinedEpicModelValidator(ISupplierDefinedEpicsService supplierDefinedEpicsService)
        {
            this.supplierDefinedEpicsService = supplierDefinedEpicsService;

            RuleFor(m => m.SelectedCapabilityId)
                .NotNull()
                .WithMessage("Select a Capability");

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
                .MustAsync(NotBeADuplicateEpic)
                .WithMessage("A supplier defined Epic with these details already exists")
                .When(m => m.SelectedCapabilityId.HasValue && m.IsActive.HasValue)
                .OverridePropertyName(
                    m => m.SelectedCapabilityId,
                    m => m.Name,
                    m => m.Description,
                    m => m.IsActive);
        }

        private async Task<bool> NotBeADuplicateEpic(AddEditSupplierDefinedEpicModel model, CancellationToken token)
        {
            _ = token;

            return !await supplierDefinedEpicsService.EpicExists(
                model.Id,
                model.SelectedCapabilityId!.Value,
                model.Name,
                model.Description,
                model.IsActive!.Value);
        }
    }
}
