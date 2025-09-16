using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.AdditionalServices;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.AdditionalServices;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators
{
    public sealed class EditAdditionalServiceDetailsModelValidator : AbstractValidator<EditAdditionalServiceDetailsModel>
    {
        private readonly IAdditionalServicesService additionalServicesService;

        public EditAdditionalServiceDetailsModelValidator(IAdditionalServicesService additionalServicesService)
        {
            this.additionalServicesService = additionalServicesService;

            RuleFor(m => m)
                .Must(NotBeADuplicateService)
                .WithMessage("Additional service name already exists. Enter a different name")
                .OverridePropertyName(m => m.Name);

            RuleFor(m => m.Name)
                .NotEmpty()
                .WithMessage("Enter an additional service name")
                .NotEqual(m => m.CatalogueItemName)
                .WithMessage("Additional service name cannot be the same as its catalogue solution");

            RuleFor(m => m.Description)
                .NotEmpty()
                .WithMessage("Enter an additional service description");
        }

        private bool NotBeADuplicateService(EditAdditionalServiceDetailsModel model)
        {
            return !additionalServicesService.AdditionalServiceExistsWithNameForSolution(
                model.Name,
                model.CatalogueItemId,
                model.Id.HasValue ? model.Id.Value : default).GetAwaiter().GetResult();
        }
    }
}
