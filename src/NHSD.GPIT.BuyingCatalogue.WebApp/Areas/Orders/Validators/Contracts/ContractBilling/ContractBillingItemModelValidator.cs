using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.ContractBilling;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators.Contracts.ContractBilling
{
    public class ContractBillingItemModelValidator : AbstractValidator<ContractBillingItemModel>
    {
        public const string NameRequiredErrorMessage = "Enter a milestone name";
        public const string PaymentTriggerRequiredErrorMessage = "Enter a milestone payment trigger";
        public const string AssociatedServiceRequiredErrorMessage = "Enter an Associated Service name";
        public const string QuantityRequiredErrorMessage = "Enter the number of units to be billed";

        public ContractBillingItemModelValidator()
        {
            RuleFor(x => x.SelectedOrderItemId)
                .NotEmpty()
                .WithMessage(AssociatedServiceRequiredErrorMessage);

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage(NameRequiredErrorMessage);

            RuleFor(x => x.PaymentTrigger)
                .NotEmpty()
                .WithMessage(PaymentTriggerRequiredErrorMessage);

            RuleFor(x => x.Quantity)
                .NotEmpty()
                .WithMessage(QuantityRequiredErrorMessage);
        }
    }
}
