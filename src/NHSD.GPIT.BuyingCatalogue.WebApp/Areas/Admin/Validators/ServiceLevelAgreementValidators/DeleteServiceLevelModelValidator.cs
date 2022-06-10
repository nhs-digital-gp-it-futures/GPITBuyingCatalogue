using System.Linq;
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
                .Must(NotBeTheOnlyEntry)
                .WithMessage("This is the only service level provided and can only be deleted if you unpublish your solution first");
        }

        private bool NotBeTheOnlyEntry(DeleteServiceLevelModel model)
        {
            var catalogueItem = solutionsService.GetSolutionWithServiceLevelAgreements(model.SolutionId).GetAwaiter().GetResult();

            var isNotPublished = catalogueItem.PublishedStatus != PublicationStatus.Published;

            return isNotPublished
                || catalogueItem.Solution.ServiceLevelAgreement.ServiceLevels.Any(sl => sl.Id != model.ServiceLevelId);
        }
    }
}
