using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.ContractBilling;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators.Contracts.ContractBilling;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Validators.Contracts.ContractBilling
{
    public static class ContractBillingItemModelValidatorTests
    {
        [Theory]
        [CommonAutoData]
        public static void Validate_Milestone_NameNull_SetsModelError(
            ContractBillingItemModel model,
            ContractBillingItemModelValidator validator)
        {
            model.Name = null;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Name)
                .WithErrorMessage(ContractBillingItemModelValidator.NameRequiredErrorMessage);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_Milestone_PaymentTriggerNull_SetsModelError(
            ContractBillingItemModel model,
            ContractBillingItemModelValidator validator)
        {
            model.PaymentTrigger = null;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.PaymentTrigger)
                .WithErrorMessage(ContractBillingItemModelValidator.PaymentTriggerRequiredErrorMessage);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_Milestone_QuantityNull_SetsModelError(
            ContractBillingItemModel model,
            ContractBillingItemModelValidator validator)
        {
            model.Quantity = null;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Quantity)
                .WithErrorMessage(ContractBillingItemModelValidator.QuantityRequiredErrorMessage);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_Milestone_SelectedOrderItemIdNull_SetsModelError(
            ContractBillingItemModel model,
            ContractBillingItemModelValidator validator)
        {
            model.SelectedOrderItemId = default(CatalogueItemId);

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.SelectedOrderItemId)
                .WithErrorMessage(ContractBillingItemModelValidator.AssociatedServiceRequiredErrorMessage);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_Valid_NoModelError(
            ContractBillingItemModel model,
            ContractBillingItemModelValidator validator)
        {
            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
