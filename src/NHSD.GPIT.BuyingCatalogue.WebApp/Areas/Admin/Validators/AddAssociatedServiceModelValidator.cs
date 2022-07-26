using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.AssociatedServices;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.AssociatedServices;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators
{
    public sealed class AddAssociatedServiceModelValidator : AbstractValidator<AddAssociatedServiceModel>
    {
        public const string EnterAssociatedServiceName = "Enter a name";
        public const string EnterAssociatedServiceDescription = "Enter a description";
        public const string EnterOrderGuidance = "Enter order guidance";
        public const string AssociatedServiceNameAlreadyExists = "Associated Service name already exists. Enter a different name";

        private readonly IAssociatedServicesService associatedServicesService;

        public AddAssociatedServiceModelValidator(IAssociatedServicesService associatedServicesService)
        {
            this.associatedServicesService = associatedServicesService;

            RuleFor(m => m.Name)
                .NotEmpty()
                .WithMessage(EnterAssociatedServiceName);

            RuleFor(m => m.Description)
                .NotEmpty()
                .WithMessage(EnterAssociatedServiceDescription);

            RuleFor(m => m.OrderGuidance)
                .NotEmpty()
                .WithMessage(EnterOrderGuidance);

            RuleFor(m => m)
                .Must(NotBeADuplicateServiceName)
                .WithMessage(AssociatedServiceNameAlreadyExists)
                .OverridePropertyName(m => m.Name);
        }

        private bool NotBeADuplicateServiceName(AddAssociatedServiceModel model)
        {
            return !associatedServicesService.AssociatedServiceExistsWithNameForSupplier(model.Name, model.SolutionId.SupplierId, default).GetAwaiter().GetResult();
        }
    }
}
