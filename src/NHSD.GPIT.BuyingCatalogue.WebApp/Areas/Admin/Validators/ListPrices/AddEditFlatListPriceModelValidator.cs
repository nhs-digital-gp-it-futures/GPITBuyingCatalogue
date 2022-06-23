using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
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

            RuleFor(m => m.Price)
                .IsValidPrice();

            RuleFor(m => m.UnitDescription)
                .NotEmpty()
                .WithMessage(SharedListPriceValidationErrors.UnitError);

            RuleFor(m => m.SelectedPublicationStatus)
                .NotNull()
                .WithMessage(SelectedPublicationStatusError);

            RuleFor(m => m.SelectedCalculationType)
                .NotNull()
                .WithMessage(SharedListPriceValidationErrors.SelectedCalculationTypeError);

            RuleFor(m => m)
                .Must(NotBeADuplicate)
                .WithMessage(SharedListPriceValidationErrors.DuplicateListPriceError)
                .Unless(m => m.SelectedProvisioningType is null || m.Price is null)
                .OverridePropertyName(
                    m => m.SelectedProvisioningType,
                    m => m.Price,
                    m => m.UnitDescription,
                    m => m.SelectedCalculationType);

            RuleFor(m => m.DeclarativeQuantityCalculationType)
                .NotNull()
                .WithMessage(SharedListPriceValidationErrors.QuantitiesCalculationError)
                .When(m => m.SelectedProvisioningType.GetValueOrDefault() is ProvisioningType.Declarative);

            RuleFor(m => m.OnDemandQuantityCalculationType)
                .NotNull()
                .WithMessage(SharedListPriceValidationErrors.QuantitiesCalculationError)
                .When(m => m.SelectedProvisioningType.GetValueOrDefault() is ProvisioningType.OnDemand);
        }

        private bool NotBeADuplicate(AddEditFlatListPriceModel model) =>
            !listPriceService.HasDuplicateFlatPrice(
                model.CatalogueItemId,
                model.CataloguePriceId,
                model.SelectedProvisioningType!.Value,
                model.SelectedCalculationType!.Value,
                model.Price!.Value,
                model.UnitDescription).GetAwaiter().GetResult();
    }
}
