using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.ModelBinders;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ServiceLevelAgreements
{
    public sealed class EditSLAContactModel : NavBaseModel
    {
        public EditSLAContactModel()
        {
        }

        public EditSLAContactModel(CatalogueItem solution, SlaContact sla, EntityFramework.Catalogue.Models.ServiceLevelAgreements slas)
        {
            ContactId = sla.Id;
            SolutionId = solution.Id;
            SolutionName = solution.Name;
            Channel = sla.Channel;
            ContactInformation = sla.ContactInformation;
            From = sla.TimeFrom;
            Until = sla.TimeUntil;

            CanDelete =
            (solution.PublishedStatus == PublicationStatus.Draft || solution.PublishedStatus == PublicationStatus.Unpublished)
            || (solution.PublishedStatus != PublicationStatus.Draft && solution.PublishedStatus != PublicationStatus.Unpublished
                && slas.Contacts.Count > 1);
        }

        public EditSLAContactModel(CatalogueItem solution, SlaContact sla)
        {
            ContactId = sla.Id;
            SolutionId = solution.Id;
            SolutionName = solution.Name;
            Channel = sla.Channel;
        }

        public int? ContactId { get; set; }

        public CatalogueItemId SolutionId { get; set; }

        [StringLength(300)]
        public string Channel { get; set; }

        [StringLength(1000)]
        public string ContactInformation { get; set; }

        [ModelBinder(BinderType = typeof(TimeInputModelBinder))]
        public DateTime? From { get; set; }

        [ModelBinder(BinderType = typeof(TimeInputModelBinder))]
        public DateTime? Until { get; set; }

        public string SolutionName { get; set; }

        public bool CanDelete { get; set; }
    }
}
