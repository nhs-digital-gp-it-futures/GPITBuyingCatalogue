using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.AssociatedServices;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.AssociatedServices;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators
{
    public class EditAssociatedServiceDetailsModelValidator : AbstractValidator<EditAssociatedServiceDetailsModel>
    {
        private readonly IAssociatedServicesService associatedServicesService;

        public EditAssociatedServiceDetailsModelValidator(IAssociatedServicesService associatedServicesService)
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

        private bool NotBeADuplicateServiceName(EditAssociatedServiceDetailsModel model)
        {
            return !associatedServicesService.AssociatedServiceExistsWithNameForSupplier(
                model.Name,
                model.SolutionId.SupplierId,
                model.Id.HasValue ? model.Id.Value : default).GetAwaiter().GetResult();
        }
    }
}
