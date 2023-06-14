using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ApplicationTypeModels.DesktopBasedModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.ApplicationType.DesktopBased
{
    public sealed class MemoryAndStorageModelValidator : AbstractValidator<MemoryAndStorageModel>
    {
        public MemoryAndStorageModelValidator()
        {
            RuleFor(m => m.SelectedMemorySize)
                .NotEmpty()
                .WithMessage("Select a minimum memory size");

            RuleFor(m => m.StorageSpace)
                .NotEmpty()
                .WithMessage("Enter storage space information");

            RuleFor(m => m.ProcessingPower)
                .NotEmpty()
                .WithMessage("Enter processing power information");
        }
    }
}
