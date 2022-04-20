using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.ListPrice;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ListPriceModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.ListPrices
{
    public class AddEditFlatListPriceModelValidator : AbstractValidator<AddEditFlatListPriceModel>
    {
        internal const string SelectedPublicationStatusError = "Select a publication status";
        internal const string DuplicateListPriceError = "A list price with these details already exists";
        private readonly IListPriceService listPriceService;

        public AddEditFlatListPriceModelValidator(IListPriceService listPriceService)
        {
            this.listPriceService = listPriceService;

            RuleFor(m => m.SelectedProvisioningType)
                .NotNull()
                .WithMessage(SharedListPriceValidationErrors.SelectedProvisioningTypeError);

            RuleFor(m => m.SelectedCalculationType)
                .NotNull()
                .WithMessage(SharedListPriceValidationErrors.SelectedCalculationTypeError);

            RuleFor(m => m.Price)
                .IsValidPrice();

            RuleFor(m => m.UnitDescription)
                .NotEmpty()
                .WithMessage(SharedListPriceValidationErrors.UnitError);

            RuleFor(m => m.SelectedPublicationStatus)
                .NotNull()
                .WithMessage(SelectedPublicationStatusError);

            RuleFor(m => m)
                .MustAsync(NotBeADuplicate)
                .WithMessage(SharedListPriceValidationErrors.DuplicateListPriceError)
                .Unless(m => m.SelectedProvisioningType is null || m.SelectedCalculationType is null || m.Price is null)
                .OverridePropertyName(
                    m => m.SelectedProvisioningType,
                    m => m.SelectedCalculationType,
                    m => m.Price,
                    m => m.UnitDescription);
        }

        private async Task<bool> NotBeADuplicate(AddEditFlatListPriceModel model, CancellationToken token)
        {
            _ = token;

            return !await listPriceService.HasDuplicateFlatPrice(
                model.CatalogueItemId,
                model.CataloguePriceId,
                model.SelectedProvisioningType!.Value,
                model.SelectedCalculationType!.Value,
                model.Price!.Value,
                model.UnitDescription);
        }
    }
}
