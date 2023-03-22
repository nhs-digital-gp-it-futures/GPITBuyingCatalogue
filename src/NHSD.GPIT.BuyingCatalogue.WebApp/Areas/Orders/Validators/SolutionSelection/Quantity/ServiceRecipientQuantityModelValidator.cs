using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.SolutionSelection.Quantity;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators.SolutionSelection.Quantity
{
    public class ServiceRecipientQuantityModelValidator : AbstractValidator<ServiceRecipientQuantityModel>
    {
        public const string ValueNotEnteredErrorMessage = "Enter a quantity for this organisation";
        public const string ValueNotNumericErrorMessage = "Quantity must be a number";
        public const string ValueNotAnIntegerErrorMessage = "Quantity must be a whole number";
        public const string ValueNegativeErrorMessage = "Quantity must be greater than zero";

        public ServiceRecipientQuantityModelValidator()
        {
            RuleFor(x => x)
                .Cascade(CascadeMode.Stop)

                .Must(HaveAValue)
                .OverridePropertyName(x => x.InputQuantity)
                .WithMessage(ValueNotEnteredErrorMessage)

                .Must(HaveANumericValue)
                .OverridePropertyName(x => x.InputQuantity)
                .WithMessage(ValueNotNumericErrorMessage)

                .Must(HaveAnIntegerValue)
                .OverridePropertyName(x => x.InputQuantity)
                .WithMessage(ValueNotAnIntegerErrorMessage)

                .Must(HaveAPositiveValue)
                .OverridePropertyName(x => x.InputQuantity)
                .WithMessage(ValueNegativeErrorMessage);
        }

        private static bool HaveAValue(ServiceRecipientQuantityModel model) => !string.IsNullOrWhiteSpace(model.InputQuantity);

        private static bool HaveAnIntegerValue(ServiceRecipientQuantityModel model) => int.TryParse(model.InputQuantity, out _);

        private static bool HaveAPositiveValue(ServiceRecipientQuantityModel model) => int.Parse(model.InputQuantity) > 0;

        private bool HaveANumericValue(ServiceRecipientQuantityModel model) => decimal.TryParse(model.InputQuantity, out _);
    }
}
