using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.AssociatedServices;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.PublicationStatusValidation
{
    public class EditAssociatedServiceModelValidator : AbstractValidator<EditAssociatedServiceModel>
    {
        public EditAssociatedServiceModelValidator()
        {
            RuleFor(m => m)
                .Must(HaveCompletedAllMandatorySections)
                .WithMessage("Complete all mandatory sections before publishing")
                .OverridePropertyName(m => m.SelectedPublicationStatus);
        }

        private bool HaveCompletedAllMandatorySections(EditAssociatedServiceModel model)
        {
            if (model.SelectedPublicationStatus != PublicationStatus.Published || model.SelectedPublicationStatus == model.AssociatedServicePublicationStatus)
                return true;

            return model.DetailsStatus == TaskProgress.Completed
                && model.ListPriceStatus == TaskProgress.Completed;
        }
    }
}
