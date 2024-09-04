using System.Linq;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.Quantities;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Validation.Quantity
{
    public class SubLocationModelValidator : AbstractValidator<SubLocationModel>
    {
        public const string ValueNotEnteredErrorMessage = "Enter all practice list sizes for {0}";

        public SubLocationModelValidator()
        {
            RuleFor(x => x)
                .Must(x => x.ServiceRecipients.All(y => !string.IsNullOrEmpty(y.InputQuantity)))
                .WithMessage(x => string.Format(ValueNotEnteredErrorMessage, x.Name))
                .OverridePropertyName(x => x);

            RuleForEach(x => x.ServiceRecipients).Cascade(CascadeMode.Continue).SetValidator(new ServiceRecipientQuantityModelValidator());
        }
    }
}
