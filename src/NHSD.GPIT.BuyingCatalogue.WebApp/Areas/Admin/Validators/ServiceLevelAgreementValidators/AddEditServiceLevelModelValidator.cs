using System.Linq;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.ServiceLevelAgreements;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ServiceLevelAgreements;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.ServiceLevelAgreementValidators
{
    public class AddEditServiceLevelModelValidator : AbstractValidator<AddEditServiceLevelModel>
    {
        private readonly IServiceLevelAgreementsService serviceLevelAgreementsService;

        public AddEditServiceLevelModelValidator(IServiceLevelAgreementsService serviceLevelAgreementsService)
        {
            this.serviceLevelAgreementsService = serviceLevelAgreementsService;

            RuleFor(m => m)
                .Must(NotBeADuplicateServiceLevel)
                .WithMessage("Service level with these details already exists")
                .Unless(m => m.CreditsApplied is null)
                .OverridePropertyName(
                    m => m.ServiceType,
                    m => m.ServiceLevel,
                    m => m.HowMeasured,
                    m => m.CreditsApplied);

            RuleFor(m => m.ServiceType)
                .NotEmpty()
                .WithMessage("Enter a type of service");

            RuleFor(m => m.ServiceLevel)
                .NotEmpty()
                .WithMessage("Enter a service level");

            RuleFor(m => m.HowMeasured)
                .NotEmpty()
                .WithMessage("Enter how service levels are measured");

            RuleFor(m => m.CreditsApplied)
                .NotNull()
                .WithMessage("Select if service credits are applied");
        }

        private bool NotBeADuplicateServiceLevel(AddEditServiceLevelModel model)
        {
            var serviceLevelAgreement = serviceLevelAgreementsService.GetServiceLevelAgreementForSolution(model.SolutionId).GetAwaiter().GetResult();

            return !serviceLevelAgreement.ServiceLevels.Any(
                sl => sl.Id != model.ServiceLevelId
                && string.Equals(sl.TypeOfService, model.ServiceType)
                && string.Equals(sl.ServiceLevel, model.ServiceLevel)
                && string.Equals(sl.HowMeasured, model.HowMeasured)
                && sl.ServiceCredits == model.CreditsApplied);
        }
    }
}
