using System.Linq;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.CatalogueSolutionRecipients;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators.Solutions
{
    public sealed class SelectSolutionServiceRecipientsModelValidator : AbstractValidator<SelectSolutionServiceRecipientsModel>
    {
        public SelectSolutionServiceRecipientsModelValidator()
        {
            RuleFor(m => m.ServiceRecipients)
                .Must(m => m.Any(sr => sr.Selected))
                .WithMessage("Select a Service Recipient")
                .OverridePropertyName("ServiceRecipients[0].Selected");
        }
    }
}
