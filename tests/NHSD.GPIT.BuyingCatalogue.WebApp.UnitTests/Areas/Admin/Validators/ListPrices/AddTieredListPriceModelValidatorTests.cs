﻿using AutoFixture.Xunit2;
using FluentValidation.TestHelper;
using Moq;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.ListPrice;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ListPriceModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.ListPrices;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators.ListPrices
{
    public static class AddTieredListPriceModelValidatorTests
    {
        [Theory]
        [CommonAutoData]
        public static void Validate_InvalidModel_SetsModelError(
            AddTieredListPriceModel model,
            AddTieredListPriceModelValidator validator)
        {
            model.SelectedProvisioningType = null;
            model.SelectedCalculationType = null;
            model.UnitDescription = null;
            model.RangeDefinition = null;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.SelectedProvisioningType)
                .WithErrorMessage(SharedListPriceValidationErrors.SelectedProvisioningTypeError);

            result.ShouldHaveValidationErrorFor(m => m.SelectedCalculationType)
                .WithErrorMessage(SharedListPriceValidationErrors.SelectedCalculationTypeError);

            result.ShouldHaveValidationErrorFor(m => m.UnitDescription)
                .WithErrorMessage(SharedListPriceValidationErrors.UnitError);

            result.ShouldHaveValidationErrorFor(m => m.RangeDefinition)
                .WithErrorMessage(AddTieredListPriceModelValidator.RangeDefinitionError);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_Duplicate_SetsModelError(
            AddTieredListPriceModel model,
            [Frozen] Mock<IListPriceService> service,
            AddTieredListPriceModelValidator validator)
        {
            model.SelectedCalculationType = EntityFramework.Catalogue.Models.CataloguePriceCalculationType.Volume;
            model.SelectedProvisioningType = EntityFramework.Catalogue.Models.ProvisioningType.Declarative;

            service.Setup(s => s.HasDuplicateTieredPrice(
                model.CatalogueItemId,
                model.CataloguePriceId,
                model.SelectedProvisioningType!.Value,
                model.SelectedCalculationType!.Value,
                model.UnitDescription,
                model.RangeDefinition)).ReturnsAsync(true);

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor("SelectedProvisioningType|SelectedCalculationType|UnitDescription|RangeDefinition")
                .WithErrorMessage(SharedListPriceValidationErrors.DuplicateListPriceError);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_Valid_NoModelError(
            AddTieredListPriceModel model,
            [Frozen] Mock<IListPriceService> service,
            AddTieredListPriceModelValidator validator)
        {
            model.SelectedCalculationType = EntityFramework.Catalogue.Models.CataloguePriceCalculationType.Volume;
            model.SelectedProvisioningType = EntityFramework.Catalogue.Models.ProvisioningType.Declarative;

            service.Setup(s => s.HasDuplicateTieredPrice(
                model.CatalogueItemId,
                model.CataloguePriceId,
                model.SelectedProvisioningType!.Value,
                model.SelectedCalculationType!.Value,
                model.UnitDescription,
                model.RangeDefinition)).ReturnsAsync(false);

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
