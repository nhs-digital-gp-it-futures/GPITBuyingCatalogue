using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ApplicationTypeModels.MobileTabletBasedModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.ApplicationType.MobileTabletBased
{
    public sealed class MemoryAndStorageModelValidator : AbstractValidator<MemoryAndStorageModel>
    {
        public MemoryAndStorageModelValidator()
        {
            RuleFor(m => m.SelectedMemorySize)
                .NotEmpty()
                .WithMessage("Select a minimum memory size");

            RuleFor(m => m.Description)
                .NotEmpty()
                .WithMessage("Enter storage space information");
        }
    }
}
