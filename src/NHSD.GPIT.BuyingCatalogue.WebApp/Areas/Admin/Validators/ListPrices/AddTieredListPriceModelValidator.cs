using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.ListPrice;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ListPriceModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.ListPrices
{
    public class AddTieredListPriceModelValidator : AbstractValidator<AddTieredListPriceModel>
    {
        internal const string RangeDefinitionError = "Enter a range definition";
        private readonly IListPriceService listPriceService;

        public AddTieredListPriceModelValidator(IListPriceService listPriceService)
        {
            this.listPriceService = listPriceService;

            RuleFor(m => m.SelectedProvisioningType)
                .NotNull()
                .WithMessage(SharedListPriceValidationErrors.SelectedProvisioningTypeError);

            RuleFor(m => m.SelectedCalculationType)
                .NotNull()
                .WithMessage(SharedListPriceValidationErrors.SelectedCalculationTypeError);

            RuleFor(m => m.UnitDescription)
                .NotEmpty()
                .WithMessage(SharedListPriceValidationErrors.UnitError);

            RuleFor(m => m.RangeDefinition)
                .NotEmpty()
                .WithMessage(RangeDefinitionError);

            RuleFor(m => m)
                .Must(NotBeADuplicate)
                .WithMessage(SharedListPriceValidationErrors.DuplicateListPriceError)
                .Unless(m => m.SelectedProvisioningType is null || m.SelectedCalculationType is null)
                .OverridePropertyName(
                    m => m.SelectedProvisioningType,
                    m => m.SelectedCalculationType,
                    m => m.UnitDescription,
                    m => m.RangeDefinition);
        }

        private bool NotBeADuplicate(AddTieredListPriceModel model)
        {
            return !listPriceService.HasDuplicateTieredPrice(
                model.CatalogueItemId,
                model.CataloguePriceId,
                model.SelectedProvisioningType!.Value,
                model.SelectedCalculationType!.Value,
                model.UnitDescription,
                model.RangeDefinition).GetAwaiter().GetResult();
        }
    }
}
