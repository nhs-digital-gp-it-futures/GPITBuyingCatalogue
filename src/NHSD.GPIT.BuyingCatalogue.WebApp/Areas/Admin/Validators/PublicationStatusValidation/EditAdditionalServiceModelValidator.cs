using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.AdditionalServices;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.PublicationStatusValidation
{
    public class EditAdditionalServiceModelValidator : AbstractValidator<EditAdditionalServiceModel>
    {
        public const string CompleteAllMandatorySections = "Complete all mandatory sections before publishing";

        public EditAdditionalServiceModelValidator()
        {
            RuleFor(m => m)
                .Must(HaveCompletedAllMandatorySections)
                .WithMessage(CompleteAllMandatorySections)
                .OverridePropertyName(m => m.SelectedPublicationStatus);
        }

        private bool HaveCompletedAllMandatorySections(EditAdditionalServiceModel model)
        {
            if (model.SelectedPublicationStatus != PublicationStatus.Published || model.SelectedPublicationStatus == model.AdditionalServicePublicationStatus)
                return true;

            return model.DetailsStatus == TaskProgress.Completed
                && model.CapabilitiesStatus == TaskProgress.Completed
                && model.ListPriceStatus == TaskProgress.Completed;
        }
    }
}
