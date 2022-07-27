using System.Linq;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.AssociatedServices;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.AssociatedServices;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.PublicationStatusValidation
{
    public class EditAssociatedServiceModelValidator : AbstractValidator<EditAssociatedServiceModel>
    {
        public const string AssociatedServiceCannotBeUnpublishedError = "This Associated Service cannot be unpublished as it is referenced by at least one solution";
        public const string UncompletedMandatorySectionsError = "Complete all mandatory sections before publishing";

        private readonly IAssociatedServicesService associatedServicesService;

        public EditAssociatedServiceModelValidator(IAssociatedServicesService associatedServicesService)
        {
            this.associatedServicesService = associatedServicesService;

            RuleFor(m => m.SelectedPublicationStatus)
                .Must(NotHaveAnyPublishedSolutionReferences)
                .WithMessage(AssociatedServiceCannotBeUnpublishedError)
                .When(m => m.SelectedPublicationStatus == PublicationStatus.Unpublished && m.SelectedPublicationStatus != m.AssociatedServicePublicationStatus)
                .Must(HaveCompletedAllMandatorySections)
                .WithMessage(UncompletedMandatorySectionsError)
                .When(m => m.SelectedPublicationStatus == PublicationStatus.Published && m.SelectedPublicationStatus != m.AssociatedServicePublicationStatus, ApplyConditionTo.CurrentValidator);
        }

        private bool HaveCompletedAllMandatorySections(EditAssociatedServiceModel model, PublicationStatus selectedPublicationStatus)
        {
            _ = selectedPublicationStatus;
            return model.DetailsStatus == TaskProgress.Completed && model.ListPriceStatus == TaskProgress.Completed;
        }

        private bool NotHaveAnyPublishedSolutionReferences(EditAssociatedServiceModel model, PublicationStatus selectedPublicationStatus)
        {
            var solutions = associatedServicesService.GetAllSolutionsForAssociatedService(model.AssociatedServiceId).GetAwaiter().GetResult();
            if (solutions.Count == 0)
                return true;

            return !solutions.Any(s => s.PublishedStatus == PublicationStatus.Published
                                   || s.PublishedStatus == PublicationStatus.InRemediation
                                   || s.PublishedStatus == PublicationStatus.Suspended);
        }
    }
}
