using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.DeliveryDates;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators.Contracts.DeliveryDates
{
    public class RecipientLocationModelValidator : AbstractValidator<KeyValuePair<string, RecipientDateModel[]>>
    {
        public RecipientLocationModelValidator()
        {
            RuleForEach(x => x.Value)
                .Cascade(CascadeMode.Continue)
                .SetValidator(new RecipientDateModelValidator());
        }
    }
}
