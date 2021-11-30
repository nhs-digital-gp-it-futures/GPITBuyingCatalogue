using System.Threading;
using System.Threading.Tasks;
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
                .MustAsync(NotBeADuplicateService)
                .WithMessage("Additional Service name already exists. Enter a different name")
                .OverridePropertyName(m => m.Name);

            RuleFor(m => m.Name)
                .NotEmpty()
                .WithMessage("Enter an Additional Service name")
                .NotEqual(m => m.CatalogueItemName)
                .WithMessage("Additional Service name cannot be the same as its Catalogue Solution");

            RuleFor(m => m.Description)
                .NotEmpty()
                .WithMessage("Enter an Additional Service description");
        }

        private async Task<bool> NotBeADuplicateService(EditAdditionalServiceDetailsModel model, CancellationToken cancellationToken)
        {
            return !await additionalServicesService.AdditionalServiceExistsWithNameForSolution(
                model.Name,
                model.CatalogueItemId,
                model.Id.HasValue ? model.Id.Value : default);
        }
    }
}
