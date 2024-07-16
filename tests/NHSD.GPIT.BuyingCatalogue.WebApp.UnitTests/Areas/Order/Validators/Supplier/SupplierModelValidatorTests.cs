using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Supplier;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators.Supplier;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Validators.Supplier
{
    public static class SupplierModelValidatorTests
    {
        [Theory]
        [MockAutoData]
        public static void Validate_ValuesMissing_ThrowsValidationError(
            SupplierModel model,
            SupplierModelValidator systemUnderTest)
        {
            model.SelectedContactId = null;

            var result = systemUnderTest.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.SelectedContactId)
                .WithErrorMessage(SupplierModelValidator.ContactNotSelectedErrorMessage);
        }
    }
}
