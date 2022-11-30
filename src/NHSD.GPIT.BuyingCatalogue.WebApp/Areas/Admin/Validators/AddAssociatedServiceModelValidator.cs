using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.AssociatedServices;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.AssociatedServices;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators
{
    public sealed class AddAssociatedServiceModelValidator : AbstractValidator<AddAssociatedServiceModel>
    {
        private readonly IAssociatedServicesService associatedServicesService;

        public AddAssociatedServiceModelValidator(IAssociatedServicesService associatedServicesService)
        {
            this.associatedServicesService = associatedServicesService;

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

        private bool NotBeADuplicateServiceName(AddAssociatedServiceModel model)
        {
            return !associatedServicesService.AssociatedServiceExistsWithNameForSupplier(model.Name, model.SolutionId.SupplierId, default).GetAwaiter().GetResult();
        }
    }
}
