using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.DeliveryDates;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators.Contracts.DeliveryDates
{
    [ExcludeFromCodeCoverage(Justification = "Configures a sub-validator to use on a property and doesn't have validation logic")]
    public class EditDatesModelValidator : AbstractValidator<EditDatesModel>
    {
        public EditDatesModelValidator()
        {
            RuleForEach(x => x.Recipients)
                .Cascade(CascadeMode.Continue)
                .SetValidator(new RecipientLocationModelValidator());
        }
    }
}
