using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.ServiceLevelAgreements;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ServiceLevelAgreements;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.ServiceLevelAgreementValidators
{
    public sealed class DeleteServiceAvailabilityTimesModelValidator : AbstractValidator<DeleteServiceAvailabilityTimesModel>
    {
        private readonly ISolutionsService solutionsService;
        private readonly IServiceLevelAgreementsService serviceLevelAgreementsService;

        public DeleteServiceAvailabilityTimesModelValidator(
            ISolutionsService solutionsService,
            IServiceLevelAgreementsService serviceLevelAgreementsService)
        {
            this.solutionsService = solutionsService;
            this.serviceLevelAgreementsService = serviceLevelAgreementsService;

            RuleFor(m => m)
                .Must(NotBeTheOnlyEntry)
                .WithMessage("These are the only service availability times provided and can only be deleted if you unpublish your solution first");
        }

        private bool NotBeTheOnlyEntry(DeleteServiceAvailabilityTimesModel model)
        {
            var solution = solutionsService.GetSolutionThin(model.SolutionId).GetAwaiter().GetResult();

            var isNotPublished = solution.PublishedStatus != PublicationStatus.Published;

            return isNotPublished
                || serviceLevelAgreementsService.GetCountOfServiceAvailabilityTimes(model.SolutionId, model.ServiceAvailabilityTimesId).GetAwaiter().GetResult() > 0;
        }
    }
}
