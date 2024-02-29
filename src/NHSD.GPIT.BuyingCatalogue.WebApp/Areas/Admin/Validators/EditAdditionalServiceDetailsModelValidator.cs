using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.AdditionalServices;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.AdditionalServices;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators
{
    public sealed class EditAdditionalServiceDetailsModelValidator : AbstractValidator<EditAdditionalServiceDetailsModel>
    {
        public const string IdRequiredErrorMessage = "Enter an Additional Service ID";
        public const string SolutionIdMismatchErrorMessage = "Additional Service ID does not contain the solution ID";
        public const string DuplicateIdErrorMessage = "An Additional Service with that ID already exists. Try a different ID";
        public const string IdFormatErrorMessage = "Additional Service ID must be in the correct format, for example 10000-001A001";

        private readonly IAdditionalServicesService additionalServicesService;

        public EditAdditionalServiceDetailsModelValidator(IAdditionalServicesService additionalServicesService)
        {
            this.additionalServicesService = additionalServicesService;

            RuleFor(m => m)
                .Must(NotBeADuplicateService)
                .WithMessage("Additional Service name already exists. Enter a different name")
                .OverridePropertyName(m => m.Name);

            RuleFor(s => s.IdDisplay)
                .NotNull()
                .WithMessage(IdRequiredErrorMessage);

            RuleFor(s => s.Id)
                .NotNull()
                .WithMessage(IdFormatErrorMessage)
                .Must((model, id) => id.GetValueOrDefault().ToString()!.Contains(model.CatalogueItemId.ToString()))
                .WithMessage(SolutionIdMismatchErrorMessage)
                .Must((model, _) => NotBeADuplicateId(model))
                .WithMessage(DuplicateIdErrorMessage)
                .When(m => !m.IsEdit)
                .OverridePropertyName(m => m.IdDisplay);

            RuleFor(m => m.Name)
                .NotEmpty()
                .WithMessage("Enter an Additional Service name")
                .NotEqual(m => m.CatalogueItemName)
                .WithMessage("Additional Service name cannot be the same as its Catalogue Solution");

            RuleFor(m => m.Description)
                .NotEmpty()
                .WithMessage("Enter an Additional Service description");
        }

        private bool NotBeADuplicateId(EditAdditionalServiceDetailsModel model)
        {
            return additionalServicesService.GetAdditionalService(model.CatalogueItemId, model.Id.GetValueOrDefault()).GetAwaiter().GetResult() is null;
        }

        private bool NotBeADuplicateService(EditAdditionalServiceDetailsModel model)
        {
            return !additionalServicesService.AdditionalServiceExistsWithNameForSolution(
                model.Name,
                model.CatalogueItemId,
                model.IsEdit ? model.Id.GetValueOrDefault() : default).GetAwaiter().GetResult();
        }
    }
}
