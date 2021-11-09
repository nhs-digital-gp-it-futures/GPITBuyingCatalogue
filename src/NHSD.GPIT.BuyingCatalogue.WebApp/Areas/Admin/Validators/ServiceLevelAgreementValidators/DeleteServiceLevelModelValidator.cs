using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ServiceLevelAgreements;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.ServiceLevelAgreementValidators
{
    public class DeleteServiceLevelModelValidator : AbstractValidator<DeleteServiceLevelModel>
    {
        private readonly ISolutionsService solutionsService;

        public DeleteServiceLevelModelValidator(ISolutionsService solutionsService)
        {
            this.solutionsService = solutionsService;

            RuleFor(m => m)
                .MustAsync(NotBeTheOnlyEntry)
                .WithMessage("This is the only service level provided and can only be deleted if you unpublish your solution first");
        }

        private async Task<bool> NotBeTheOnlyEntry(DeleteServiceLevelModel model, CancellationToken token)
        {
            var catalogueItem = await solutionsService.GetSolution(model.SolutionId);

            var isNotPublished = catalogueItem.PublishedStatus != PublicationStatus.Published;

            return isNotPublished
                || catalogueItem.Solution.ServiceLevelAgreement.ServiceLevels.Any(sl => sl.Id != model.ServiceLevelId);
        }
    }
}
