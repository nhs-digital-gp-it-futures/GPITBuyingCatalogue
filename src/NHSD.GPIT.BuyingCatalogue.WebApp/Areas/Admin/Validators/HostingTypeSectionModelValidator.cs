using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators
{
    public sealed class HostingTypeSectionModelValidator : AbstractValidator<HostingTypeSectionModel>
    {
        internal const string OneHostingTypeRequiredMessage = "Add at least one hosting type";

        public HostingTypeSectionModelValidator()
        {
            RuleFor(m => m.ExistingHostingTypesCount)
                .GreaterThan(0)
                .WithMessage(OneHostingTypeRequiredMessage);
        }
    }
}
