using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.Quantities;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Validation.Quantity
{
    public class SelectOrderItemQuantityModelValidator : AbstractValidator<SelectOrderItemQuantityModel>
    {
        public const string QuantityNotEnteredErrorMessage = "Enter a quantity";
        public const string QuantityNotANumberErrorMessage = "Quantity must be a number";
        public const string QuantityNotAWholeNumberErrorMessage = "Quantity must be a whole number";
        public const string QuantityNegativeErrorMessage = "Quantity must be greater than zero";

        public SelectOrderItemQuantityModelValidator()
        {
            RuleFor(x => x.Quantity)
                .Cascade(CascadeMode.Stop)

                .Must(NotBeNull)
                .WithMessage(QuantityNotEnteredErrorMessage)

                .Must(BeANumber)
                .WithMessage(QuantityNotANumberErrorMessage)

                .Must(BeAWholeNumber)
                .WithMessage(QuantityNotAWholeNumberErrorMessage)

                .Must(NotBeNegative)
                .WithMessage(QuantityNegativeErrorMessage);
        }

        private static bool BeANumber(string quantity) => decimal.TryParse(quantity, out _);

        private static bool BeAWholeNumber(string quantity) => int.TryParse(quantity, out _);

        private static bool NotBeNegative(string quantity) => int.Parse(quantity) > 0;

        private static bool NotBeNull(string quantity) => !string.IsNullOrWhiteSpace(quantity);
    }
}
