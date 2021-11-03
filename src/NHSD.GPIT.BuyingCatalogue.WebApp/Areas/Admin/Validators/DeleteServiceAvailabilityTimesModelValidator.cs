using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.ServiceLevelAgreements;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ServiceLevelAgreements;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators
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
                .MustAsync(NotBeTheOnlyEntry)
                .WithMessage("These are the only service availability times provided and can only be deleted if you unpublish your solution first");
        }

        private async Task<bool> NotBeTheOnlyEntry(DeleteServiceAvailabilityTimesModel model, CancellationToken token)
        {
            var solution = await solutionsService.GetSolution(model.SolutionId);

            var isNotPublished = solution.PublishedStatus != PublicationStatus.Published;

            return isNotPublished || ((await serviceLevelAgreementsService.GetCountOfServiceAvailabilityTimes(model.ServiceAvailabilityTimesId)) > 0);
        }
    }
}
