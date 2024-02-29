using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.AssociatedServices;
using NHSD.GPIT.BuyingCatalogue.Services.AdditionalServices;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.AdditionalServices;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.AssociatedServices;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators
{
    public sealed class AddAssociatedServiceModelValidator : AbstractValidator<AddAssociatedServiceModel>
    {
        public const string IdRequiredErrorMessage = "Enter an Associated Service ID";
        public const string SupplierMismatchErrorMessage = "Associated Service ID does not contain the supplier ID";
        public const string DuplicateIdErrorMessage = "An Associated Service with that ID already exists. Try a different ID";
        public const string IdFormatErrorMessage = "Associated Service ID must be in the correct format, for example 10000-S-001";

        private readonly IAssociatedServicesService associatedServicesService;

        public AddAssociatedServiceModelValidator(IAssociatedServicesService associatedServicesService)
        {
            this.associatedServicesService = associatedServicesService;

            RuleFor(s => s.IdDisplay)
                .NotNull()
                .WithMessage(IdRequiredErrorMessage);

            RuleFor(s => s.Id)
                .NotNull()
                .WithMessage(IdFormatErrorMessage)
                .Must((model, id) => id.GetValueOrDefault().SupplierId == model.SolutionId.SupplierId)
                .WithMessage(SupplierMismatchErrorMessage)
                .Must(NotBeADuplicateId)
                .WithMessage(DuplicateIdErrorMessage)
                .OverridePropertyName(m => m.IdDisplay);

            RuleFor(m => m.Name)
                .NotEmpty()
                .WithMessage("Enter a name");

            RuleFor(m => m.Description)
                .NotEmpty()
                .WithMessage("Enter a description");

            RuleFor(m => m.OrderGuidance)
                .NotEmpty()
                .WithMessage("Enter order guidance");

            RuleFor(m => m)
                .Must(NotBeADuplicateServiceName)
                .WithMessage("Associated Service name already exists. Enter a different name")
                .OverridePropertyName(m => m.Name);
        }

        private bool NotBeADuplicateId(CatalogueItemId? id)
        {
            return associatedServicesService.GetAssociatedService(id.GetValueOrDefault()).GetAwaiter().GetResult() is null;
        }

        private bool NotBeADuplicateServiceName(AddAssociatedServiceModel model)
        {
            return !associatedServicesService.AssociatedServiceExistsWithNameForSupplier(model.Name, model.SolutionId.SupplierId, default).GetAwaiter().GetResult();
        }
    }
}
