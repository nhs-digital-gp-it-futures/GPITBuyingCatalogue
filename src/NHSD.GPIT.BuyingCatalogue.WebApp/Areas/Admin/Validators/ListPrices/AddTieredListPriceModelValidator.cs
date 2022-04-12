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
        internal const string SelectedProvisioningTypeError = "Select a provisioning type";
        internal const string SelectedCalculationTypeError = "Select a calculation type";
        internal const string UnitError = "Enter a unit";
        internal const string RangeDefinitionError = "Enter a range definition";
        internal const string DuplicateListPriceError = "A list price with these details already exists";

        private readonly IListPriceService listPriceService;

        public AddTieredListPriceModelValidator(IListPriceService listPriceService)
        {
            this.listPriceService = listPriceService;

            RuleFor(m => m.SelectedProvisioningType)
                .NotNull()
                .WithMessage(SelectedProvisioningTypeError);

            RuleFor(m => m.SelectedCalculationType)
                .NotNull()
                .WithMessage(SelectedCalculationTypeError);

            RuleFor(m => m.UnitDescription)
                .NotEmpty()
                .WithMessage(UnitError);

            RuleFor(m => m.RangeDefinition)
                .NotEmpty()
                .WithMessage(RangeDefinitionError);

            RuleFor(m => m)
                .MustAsync(NotBeADuplicate)
                .WithMessage(DuplicateListPriceError)
                .Unless(m => m.SelectedProvisioningType is null || m.SelectedCalculationType is null)
                .OverridePropertyName(
                    m => m.SelectedProvisioningType,
                    m => m.SelectedCalculationType,
                    m => m.UnitDescription,
                    m => m.RangeDefinition);
        }

        private async Task<bool> NotBeADuplicate(AddTieredListPriceModel model, CancellationToken token)
        {
            _ = token;

            return !await listPriceService.HasDuplicateTieredPrice(
                model.CatalogueItemId,
                model.CataloguePriceId,
                model.SelectedProvisioningType!.Value,
                model.SelectedCalculationType!.Value,
                model.UnitDescription,
                model.RangeDefinition);
        }
    }
}
