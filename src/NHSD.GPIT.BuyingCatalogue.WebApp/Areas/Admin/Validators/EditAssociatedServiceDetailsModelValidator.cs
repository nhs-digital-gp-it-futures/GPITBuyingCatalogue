using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.AssociatedServices;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.AssociatedServices;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators
{
    public class EditAssociatedServiceDetailsModelValidator : AbstractValidator<EditAssociatedServiceDetailsModel>
    {
        public const string NameError = "Enter a name";
        public const string DescriptionError = "Enter a description";
        public const string OrderGuidanceError = "Enter order guidance";
        public const string NameAlreadyExistsError = "Associated Service name already exists. Enter a different name";

        private readonly IAssociatedServicesService associatedServicesService;

        public EditAssociatedServiceDetailsModelValidator(IAssociatedServicesService associatedServicesService)
        {
            this.associatedServicesService = associatedServicesService;

            RuleFor(m => m.Name)
                .NotEmpty()
                .WithMessage(NameError);

            RuleFor(m => m.Description)
                .NotEmpty()
                .WithMessage(DescriptionError);

            RuleFor(m => m.OrderGuidance)
                .NotEmpty()
                .WithMessage(OrderGuidanceError);

            RuleFor(m => m)
                .Must(NotBeADuplicateServiceName)
                .WithMessage(NameAlreadyExistsError)
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
