using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.AssociatedServices;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.AssociatedServices;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.PublicationStatusValidation
{
    public class EditAssociatedServiceModelValidator : AbstractValidator<EditAssociatedServiceModel>
    {
        private readonly IAssociatedServicesService associatedServicesService;

        public EditAssociatedServiceModelValidator(IAssociatedServicesService associatedServicesService)
        {
            this.associatedServicesService = associatedServicesService;

            RuleFor(m => m.SelectedPublicationStatus)
                .MustAsync(NotHaveAnyPublishedSolutionReferences)
                .WithMessage("This Associated Service cannot be unpublished as it is referenced by another solution")
                .When(m => m.SelectedPublicationStatus == PublicationStatus.Unpublished && m.SelectedPublicationStatus != m.AssociatedServicePublicationStatus)
                .Must(HaveCompletedAllMandatorySections)
                .WithMessage("Complete all mandatory sections before publishing")
                .When(m => m.SelectedPublicationStatus == PublicationStatus.Published && m.SelectedPublicationStatus != m.AssociatedServicePublicationStatus, ApplyConditionTo.CurrentValidator);
        }

        private bool HaveCompletedAllMandatorySections(EditAssociatedServiceModel model, PublicationStatus selectedPublicationStatus)
        {
            _ = selectedPublicationStatus;
            return model.DetailsStatus == TaskProgress.Completed && model.ListPriceStatus == TaskProgress.Completed;
        }

        private async Task<bool> NotHaveAnyPublishedSolutionReferences(EditAssociatedServiceModel model, PublicationStatus selectedPublicationStatus, CancellationToken cancellationToken)
        {
            var solutions = await associatedServicesService.GetAllSolutionsForAssociatedService(model.SolutionId, model.AssociatedServiceId);
            if (solutions.Count == 0)
                return true;

            return !solutions.Any(s => s.PublishedStatus == PublicationStatus.Published
                                   || s.PublishedStatus == PublicationStatus.InRemediation
                                   || s.PublishedStatus == PublicationStatus.Suspended);
        }
    }
}
