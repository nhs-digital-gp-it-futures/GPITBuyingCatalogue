using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Supplier;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators.Supplier;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Validators.Supplier
{
    public static class SelectSupplierModelValidatorTests
    {
        [Theory]
        [MockInlineAutoData(null, OrderTypeEnum.Solution)]
        [MockInlineAutoData("", OrderTypeEnum.Solution)]
        [MockInlineAutoData(" ", OrderTypeEnum.Solution)]
        [MockInlineAutoData(null, OrderTypeEnum.AssociatedServiceOther)]
        [MockInlineAutoData("", OrderTypeEnum.AssociatedServiceOther)]
        [MockInlineAutoData(" ", OrderTypeEnum.AssociatedServiceOther)]
        public static void Validate_ValuesMissing_ThrowsValidationError_Search(
            string supplierId,
            OrderTypeEnum orderType,
            SelectSupplierModel model,
            SelectSupplierModelValidator systemUnderTest)
        {
            model.SelectedSupplierId = supplierId;
            model.OrderType = new OrderType(orderType);

            var result = systemUnderTest.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.SelectedSupplierId)
                .WithErrorMessage(SelectSupplierModelValidator.SupplierSearchMissingErrorMessage);
        }

        [Theory]
        [MockInlineAutoData(null, OrderTypeEnum.AssociatedServiceSplit)]
        [MockInlineAutoData("", OrderTypeEnum.AssociatedServiceSplit)]
        [MockInlineAutoData(" ", OrderTypeEnum.AssociatedServiceSplit)]
        [MockInlineAutoData(null, OrderTypeEnum.AssociatedServiceMerger)]
        [MockInlineAutoData("", OrderTypeEnum.AssociatedServiceMerger)]
        [MockInlineAutoData(" ", OrderTypeEnum.AssociatedServiceMerger)]
        public static void Validate_ValuesMissing_ThrowsValidationError_Select(
            string supplierId,
            OrderTypeEnum orderType,
            SelectSupplierModel model,
            SelectSupplierModelValidator systemUnderTest)
        {
            model.SelectedSupplierId = supplierId;
            model.OrderType = new OrderType(orderType);

            var result = systemUnderTest.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.SelectedSupplierId)
                .WithErrorMessage(SelectSupplierModelValidator.SupplierSelectMissingErrorMessage);
        }
    }
}
