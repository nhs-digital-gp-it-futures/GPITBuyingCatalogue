using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.SolutionSelection.Quantity;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators.SolutionSelection.Quantity
{
    public class ServiceRecipientQuantityModelValidator : AbstractValidator<ServiceRecipientQuantityModel>
    {
        public const string ValueNotEnteredErrorMessage = "Enter a practice list size as we do not have one for this organisation";
        public const string ValueNotNumericErrorMessage = "Practice list size must be a number";
        public const string ValueNotAnIntegerErrorMessage = "Practice list size must be a whole number";
        public const string ValueNegativeErrorMessage = "Practice list size must be greater than zero";

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

        private static bool HaveAValue(ServiceRecipientQuantityModel model)
        {
            return model.Quantity > 0
                || !string.IsNullOrWhiteSpace(model.InputQuantity);
        }

        private static bool HaveAnIntegerValue(ServiceRecipientQuantityModel model)
        {
            return model.Quantity > 0
                || int.TryParse(model.InputQuantity, out _);
        }

        private static bool HaveAPositiveValue(ServiceRecipientQuantityModel model)
        {
            return model.Quantity > 0
                || int.Parse(model.InputQuantity) > 0;
        }

        private bool HaveANumericValue(ServiceRecipientQuantityModel model)
        {
            return model.Quantity > 0
                || decimal.TryParse(model.InputQuantity, out _);
        }
    }
}
