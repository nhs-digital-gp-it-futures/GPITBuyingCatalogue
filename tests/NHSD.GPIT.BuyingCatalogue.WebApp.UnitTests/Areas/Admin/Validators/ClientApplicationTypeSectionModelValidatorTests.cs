using AutoFixture.Xunit2;
using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ClientApplicationTypeModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators
{
    public static class ClientApplicationTypeSectionModelValidatorTests
    {
        [Theory]
        [CommonAutoData]
        public static void Validate_NoExistingHostingTypes_HasError(
            [Frozen] CatalogueItem catalogueItem,
            [Frozen] Solution solution,
            ApplicationTypeSectionModel model,
            ClientApplicationTypeSectionModelValidator validator)
        {
            // CatalogueItem and Solution must be frozen so that a catalogue item instance with solution is passed
            // to the ClientApplicationTypeSectionModel constructor
            _ = catalogueItem;
            _ = solution;

            model.ExistingApplicationTypesCount = 0;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.ExistingApplicationTypesCount)
                .WithErrorMessage(ClientApplicationTypeSectionModelValidator.OneApplicationTypeRequiredMessage);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_ExistingHostingTypes_DoesNotHaveError(
            [Frozen] CatalogueItem catalogueItem,
            [Frozen] Solution solution,
            ApplicationTypeSectionModel model,
            ClientApplicationTypeSectionModelValidator validator)
        {
            // CatalogueItem and Solution must be frozen so that a catalogue item instance with solution is passed
            // to the ClientApplicationTypeSectionModel constructor
            _ = catalogueItem;
            _ = solution;

            var result = validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(m => m.ExistingApplicationTypesCount);
        }
    }
}
