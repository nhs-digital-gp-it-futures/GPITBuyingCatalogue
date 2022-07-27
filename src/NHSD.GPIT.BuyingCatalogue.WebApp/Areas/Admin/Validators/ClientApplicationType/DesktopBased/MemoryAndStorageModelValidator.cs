using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ClientApplicationTypeModels.DesktopBasedModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.ClientApplicationType.DesktopBased
{
    public sealed class MemoryAndStorageModelValidator : AbstractValidator<MemoryAndStorageModel>
    {
        public const string SelectMinimumMemorySizeError = "Select a minimum memory size";

        public const string StorageSpaceInformationError = "Enter storage space information";

        public const string ProcessingPowerInformationError = "Enter processing power information";

        public MemoryAndStorageModelValidator()
        {
            RuleFor(m => m.SelectedMemorySize)
                .NotEmpty()
                .WithMessage(SelectMinimumMemorySizeError);

            RuleFor(m => m.StorageSpace)
                .NotEmpty()
                .WithMessage(StorageSpaceInformationError);

            RuleFor(m => m.ProcessingPower)
                .NotEmpty()
                .WithMessage(ProcessingPowerInformationError);
        }
    }
}
