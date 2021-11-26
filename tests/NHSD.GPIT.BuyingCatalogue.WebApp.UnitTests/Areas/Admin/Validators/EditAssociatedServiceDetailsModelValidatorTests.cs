using AutoFixture.Xunit2;
using FluentValidation.TestHelper;
using Moq;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.AssociatedServices;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.AssociatedServices;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators
{
    public static class EditAssociatedServiceDetailsModelValidatorTests
    {
        [Theory]
        [CommonAutoData]
        public static void Validate_DuplicateNameForSupplier_SetsModelError(
            [Frozen] Mock<IAssociatedServicesService> associatedServicesService,
            EditAssociatedServiceDetailsModel model,
            EditAssociatedServiceDetailsModelValidator validator)
        {
            associatedServicesService.Setup(s => s.AssociatedServiceExistsWithNameForSupplier(model.Name, model.SolutionId.SupplierId, default))
                .ReturnsAsync(true);

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Name)
                .WithErrorMessage("Associated Service name already exists. Enter a different name");
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_ValidName_NoModelError(
            [Frozen] Mock<IAssociatedServicesService> associatedServicesService,
            EditAssociatedServiceDetailsModel model,
            EditAssociatedServiceDetailsModelValidator validator)
        {
            associatedServicesService.Setup(s => s.AssociatedServiceExistsWithNameForSupplier(model.Name, model.SolutionId.SupplierId, default))
                .ReturnsAsync(false);

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
