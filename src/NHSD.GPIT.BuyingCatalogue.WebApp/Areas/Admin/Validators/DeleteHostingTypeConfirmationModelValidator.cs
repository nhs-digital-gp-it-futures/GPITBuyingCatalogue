using System.Linq;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.HostingTypeModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators
{
    public sealed class DeleteHostingTypeConfirmationModelValidator : AbstractValidator<DeleteHostingTypeConfirmationModel>
    {
        internal const string ErrorMessage = "This is the only hosting type you've added and it can only be deleted if you unpublish your solution first";

        private readonly ISolutionsService solutionsService;

        public DeleteHostingTypeConfirmationModelValidator(ISolutionsService solutionsService)
        {
            this.solutionsService = solutionsService;

            RuleFor(m => m)
                .Must(NotBeTheOnlyHostingType)
                .WithMessage(ErrorMessage);
        }

        private bool NotBeTheOnlyHostingType(DeleteHostingTypeConfirmationModel model)
        {
            var solution = solutionsService.GetSolutionThin(model.SolutionId).GetAwaiter().GetResult();
            var solutionHostingOptions = solution.Solution.Hosting.AvailableHosting();

            return solutionHostingOptions.Count > 1
                || !solutionHostingOptions.Contains(model.HostingType)
                || !solution.IsBrowsable;
        }
    }
}
