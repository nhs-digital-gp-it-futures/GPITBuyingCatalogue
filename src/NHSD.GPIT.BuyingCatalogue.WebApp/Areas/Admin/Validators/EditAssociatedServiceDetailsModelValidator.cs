using System.Linq;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.AssociatedServices;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.AssociatedServices;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators
{
    public class EditAssociatedServiceDetailsModelValidator : AbstractValidator<EditAssociatedServiceDetailsModel>
    {
        private const string MultipleSolutions = "You cannot make these changes as the following solutions reference this service and already have an Associated Service of this type:";
        private const string SingleSolution = "You cannot make these changes as the following solution references this service and already has an Associated Service of this type:";
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

            RuleFor(m => m.SolutionMergerAndSplits)
                .Must((model, solutions, context) =>
                {
                    var problems = solutions.Select(x => x.With(model.PracticeReorganisation)).Where(x => x.IsNotValid);
                    context.MessageFormatter.AppendArgument("Solutions", string.Join(", ", problems.Select(p => p.SolutionName)));
                    context.MessageFormatter.AppendArgument("Message", problems.Count() > 1 ? MultipleSolutions : SingleSolution);
                    return !problems.Any();
                })
                .WithMessage((_, y) => "{Message} {Solutions}")
                .OverridePropertyName("practice-reorganisation");

            RuleFor(m => m)
                .Must(model => model.PracticeReorganisation != PracticeReorganisationTypeEnum.None ?
                    HaveCorrectProvisioningAndCalculationTypes(model)
                    && NotHaveTieredPrices(model)
                    : true)
                .WithMessage("This Associated Service has invalid price types for mergers and splits.You must edit the price types first")
                .OverridePropertyName("practice-reorganisation");
        }

        private bool NotBeADuplicateServiceName(EditAssociatedServiceDetailsModel model)
        {
            return !associatedServicesService.AssociatedServiceExistsWithNameForSupplier(
            model.Name,
            model.SupplierId,
            model.Id.HasValue ? model.Id.Value : default).GetAwaiter().GetResult();
        }

        private bool HaveCorrectProvisioningAndCalculationTypes(EditAssociatedServiceDetailsModel model) =>
            model.CataloguePrices.All(x =>
            x.ProvisioningType == (ProvisioningType.Declarative | ProvisioningType.PerServiceRecipient)
            && x.CataloguePriceCalculationType == CataloguePriceCalculationType.Volume);

        private bool NotHaveTieredPrices(EditAssociatedServiceDetailsModel model) =>
            model.CataloguePrices.All(x =>
            x.CataloguePriceType != CataloguePriceType.Tiered);
    }
}
