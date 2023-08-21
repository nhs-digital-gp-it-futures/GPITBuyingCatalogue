using System;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.Pricing;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Validation.Pricing
{
    public class PricingTierModelValidator : AbstractValidator<PricingTierModel>
    {
        public const string PriceNotEnteredErrorMessage = "Enter a price";
        public const string PriceNotNumericErrorMessage = "Price must be a number";
        public const string PriceNegativeErrorMessage = "Price cannot be negative";
        public const string PriceNotWithinFourDecimalPlacesErrorMessage = "Price must be to a maximum of 4 decimal places";
        public const string PriceHigherThanListPriceErrorMessage = "Price cannot be higher than list price";

        private const string AgreedPricePropertyName = "AgreedPrice";

        public PricingTierModelValidator()
        {
            RuleFor(x => x)
                .Cascade(CascadeMode.Stop)

                .Must(NotBeNull)
                .OverridePropertyName(AgreedPricePropertyName)
                .WithMessage(PriceNotEnteredErrorMessage)

                .Must(BeNumeric)
                .OverridePropertyName(AgreedPricePropertyName)
                .WithMessage(PriceNotNumericErrorMessage)

                .Must(NotBeNegative)
                .OverridePropertyName(AgreedPricePropertyName)
                .WithMessage(PriceNegativeErrorMessage)

                .Must(BeWithinFourDecimalPlaces)
                .OverridePropertyName(AgreedPricePropertyName)
                .WithMessage(PriceNotWithinFourDecimalPlacesErrorMessage)

                .Must(NotBeHigherThanListPrice)
                .OverridePropertyName(AgreedPricePropertyName)
                .WithMessage(x => $"{PriceHigherThanListPriceErrorMessage} (£{x.ListPrice:#,##0.00##})");
        }

        private static bool BeWithinFourDecimalPlaces(PricingTierModel model)
        {
            var price = decimal.Parse(model.AgreedPrice);

            return price == Math.Round(price, 4);
        }

        private static bool NotBeHigherThanListPrice(PricingTierModel model)
        {
            return decimal.Parse(model.AgreedPrice) <= model.ListPrice;
        }

        private static bool NotBeNegative(PricingTierModel model) => decimal.Parse(model.AgreedPrice) >= 0M;

        private static bool NotBeNull(PricingTierModel model) => !string.IsNullOrWhiteSpace(model.AgreedPrice);

        private bool BeNumeric(PricingTierModel model) => decimal.TryParse(model.AgreedPrice, out _);
    }
}
