using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ClientApplicationTypeModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators
{
    public sealed class ClientApplicationTypeSectionModelValidator : AbstractValidator<ApplicationTypeSectionModel>
    {
        internal const string OneApplicationTypeRequiredMessage = "Add an application type";

        public ClientApplicationTypeSectionModelValidator()
        {
            RuleFor(m => m.ExistingApplicationTypesCount)
                .GreaterThan(0)
                .WithMessage(OneApplicationTypeRequiredMessage);
        }
    }
}
