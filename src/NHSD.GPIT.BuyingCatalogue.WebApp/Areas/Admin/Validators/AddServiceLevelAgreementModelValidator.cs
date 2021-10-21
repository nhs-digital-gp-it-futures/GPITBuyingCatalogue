using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ServiceLevelAgreements;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators
{
    public class AddServiceLevelAgreementModelValidator : AbstractValidator<AddSlaTypeModel>
    {
        public AddServiceLevelAgreementModelValidator()
        {
            RuleFor(s => s.SlaLevel)
                .NotNull()
                .WithMessage("Select a type of Catalogue Solution");
        }
    }
}
