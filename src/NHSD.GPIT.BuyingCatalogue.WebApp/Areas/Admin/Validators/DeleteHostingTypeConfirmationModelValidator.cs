using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.HostingTypeModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators
{
    public sealed class DeleteHostingTypeConfirmationModelValidator : AbstractValidator<DeleteHostingTypeConfirmationModel>
    {
        internal const string ErrorMessage = "This is the only hosting type you've added and it can only be deleted if you unpublish your solution first";

        private static readonly PublicationStatus[] ActiveStatuses = new[]
        {
            PublicationStatus.Published,
            PublicationStatus.InRemediation,
        };

        private readonly ISolutionsService solutionsService;

        public DeleteHostingTypeConfirmationModelValidator(ISolutionsService solutionsService)
        {
            this.solutionsService = solutionsService;

            RuleFor(m => m)
                .Cascade(CascadeMode.Stop)
                .MustAsync(NotBeTheOnlyHostingType)
                .WithMessage(ErrorMessage);
        }

        private async Task<bool> NotBeTheOnlyHostingType(DeleteHostingTypeConfirmationModel model, CancellationToken token)
        {
            var solution = await solutionsService.GetSolution(model.SolutionId);
            var solutionHostingOptions = solution.Solution.Hosting.AvailableHosting();
            if (solutionHostingOptions.Count == 1 &&
                solutionHostingOptions.Contains(model.HostingType) &&
                ActiveStatuses.Contains(solution.PublishedStatus))
            {
                return false;
            }

            return true;
        }
    }
}
