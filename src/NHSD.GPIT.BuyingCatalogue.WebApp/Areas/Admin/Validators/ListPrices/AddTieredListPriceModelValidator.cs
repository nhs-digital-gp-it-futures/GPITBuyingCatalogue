﻿using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.ListPrice;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ListPriceModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.ListPrices
{
    public class AddTieredListPriceModelValidator : AbstractValidator<AddTieredListPriceModel>
    {
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
                .WithMessage(SharedListPriceValidationErrors.RangeDefinitionError);

            RuleFor(m => m)
                .Must(NotBeADuplicate)
                .WithMessage(SharedListPriceValidationErrors.DuplicateListPriceError)
                .Unless(m => m.SelectedProvisioningType is null || m.SelectedCalculationType is null)
                .OverridePropertyName(
                    m => m.SelectedProvisioningType,
                    m => m.SelectedCalculationType,
                    m => m.UnitDescription,
                    m => m.RangeDefinition);

            RuleFor(m => m.DeclarativeQuantityCalculationType)
                .NotNull()
                .WithMessage(SharedListPriceValidationErrors.QuantitiesCalculationError)
                .When(m => m.SelectedProvisioningType.GetValueOrDefault() is ProvisioningType.Declarative);

            RuleFor(m => m.OnDemandQuantityCalculationType)
                .NotNull()
                .WithMessage(SharedListPriceValidationErrors.QuantitiesCalculationError)
                .When(m => m.SelectedProvisioningType.GetValueOrDefault() is ProvisioningType.OnDemand);
        }

        private static CatalogueItemId GetCatalogueItemId(AddTieredListPriceModel model) =>
            model.CatalogueItemType == CatalogueItemType.Solution
                ? model.CatalogueItemId
                : model.ServiceId!.Value;

        private bool NotBeADuplicate(AddTieredListPriceModel model) =>
            !listPriceService.HasDuplicateTieredPrice(
                GetCatalogueItemId(model),
                model.CataloguePriceId,
                model.SelectedProvisioningType!.Value,
                model.SelectedCalculationType!.Value,
                model.UnitDescription,
                model.RangeDefinition).GetAwaiter().GetResult();
    }
}
