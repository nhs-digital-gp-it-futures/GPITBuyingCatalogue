using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.AdditionalServices;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.AdditionalServices;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators
{
    public sealed class EditAdditionalServiceDetailsModelValidator : AbstractValidator<EditAdditionalServiceDetailsModel>
    {
        public const string AdditionalServiceNameAlreadyExists = "Additional Service name already exists. Enter a different name";
        public const string EnterAdditionalServiceName = "Enter an Additional Service name";
        public const string AdditionalServiceNameSameAsCatalogueSolutionName = "Additional Service name cannot be the same as its Catalogue Solution";
        public const string EnterAdditionalServiceDescription = "Enter an Additional Service description";

        private readonly IAdditionalServicesService additionalServicesService;

        public EditAdditionalServiceDetailsModelValidator(IAdditionalServicesService additionalServicesService)
        {
            this.additionalServicesService = additionalServicesService;

            RuleFor(m => m)
                .Must(NotBeADuplicateService)
                .WithMessage(AdditionalServiceNameAlreadyExists)
                .OverridePropertyName(m => m.Name);

            RuleFor(m => m.Name)
                .NotEmpty()
                .WithMessage(EnterAdditionalServiceName)
                .NotEqual(m => m.CatalogueItemName)
                .WithMessage(AdditionalServiceNameSameAsCatalogueSolutionName);

            RuleFor(m => m.Description)
                .NotEmpty()
                .WithMessage(EnterAdditionalServiceDescription);
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
