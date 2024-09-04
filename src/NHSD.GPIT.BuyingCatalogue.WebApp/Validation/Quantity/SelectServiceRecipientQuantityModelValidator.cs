using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.Quantities;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Validation.Quantity
{
    public class SelectServiceRecipientQuantityModelValidator : AbstractValidator<SelectServiceRecipientQuantityModel>
    {
        public SelectServiceRecipientQuantityModelValidator()
        {
            RuleForEach(x => x.SubLocations).Cascade(CascadeMode.Continue).SetValidator(new SubLocationModelValidator());
        }
    }
}
