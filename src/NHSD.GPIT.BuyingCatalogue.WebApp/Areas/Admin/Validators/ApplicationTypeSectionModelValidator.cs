using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ApplicationTypeModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators
{
    public sealed class ApplicationTypeSectionModelValidator : AbstractValidator<ApplicationTypeSectionModel>
    {
        internal const string OneApplicationTypeRequiredMessage = "Add an application type";

        public ApplicationTypeSectionModelValidator()
        {
            RuleFor(m => m.ExistingApplicationTypesCount)
                .GreaterThan(0)
                .WithMessage(OneApplicationTypeRequiredMessage);
        }
    }
}
