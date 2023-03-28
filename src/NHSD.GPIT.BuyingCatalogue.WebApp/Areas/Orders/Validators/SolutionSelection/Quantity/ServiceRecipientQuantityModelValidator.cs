using System.Collections.Generic;
using FluentValidation;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.SolutionSelection.Quantity;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators.SolutionSelection.Quantity
{
    public class ServiceRecipientQuantityModelValidator : AbstractValidator<ServiceRecipientQuantityModel>
    {
        public const string ValueNotEnteredErrorMessage = "Enter a practice list size for {0}";
        public const string ValueNotNumericErrorMessage = "Practice list size for {0} must be a number";
        public const string ValueNotAnIntegerErrorMessage = "Practice list size for {0} must be a whole number";
        public const string ValueNegativeErrorMessage = "Practice list size for {0} must be greater than zero";

        public ServiceRecipientQuantityModelValidator()
        {
            RuleFor(x => x)
                .Cascade(CascadeMode.Stop)

                .Must(HaveAValue)
                .OverridePropertyName(x => x.InputQuantity)
                .WithMessage(model => string.Format(ValueNotEnteredErrorMessage, model.Name))

                .Must(HaveANumericValue)
                .OverridePropertyName(x => x.InputQuantity)
                .WithMessage(model => string.Format(ValueNotNumericErrorMessage, model.Name))

                .Must(HaveAnIntegerValue)
                .OverridePropertyName(x => x.InputQuantity)
                .WithMessage(model => string.Format(ValueNotAnIntegerErrorMessage, model.Name))

                .Must(HaveAPositiveValue)
                .OverridePropertyName(x => x.InputQuantity)
                .WithMessage(model => string.Format(ValueNegativeErrorMessage, model.Name));
        }

        private static bool HaveAValue(ServiceRecipientQuantityModel model) => !string.IsNullOrWhiteSpace(model.InputQuantity);

        private static bool HaveAnIntegerValue(ServiceRecipientQuantityModel model) => int.TryParse(model.InputQuantity, out _);

        private static bool HaveAPositiveValue(ServiceRecipientQuantityModel model) => int.Parse(model.InputQuantity) > 0;

        private bool HaveANumericValue(ServiceRecipientQuantityModel model) => decimal.TryParse(model.InputQuantity, out _);
    }
}
