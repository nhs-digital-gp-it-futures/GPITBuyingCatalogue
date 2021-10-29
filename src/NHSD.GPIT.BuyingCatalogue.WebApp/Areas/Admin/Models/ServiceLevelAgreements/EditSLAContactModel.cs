using System;
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

        public EditSLAContactModel(CatalogueItem solution, SlaContact sla, bool canDelete)
        {
            ContactId = sla.Id;
            SolutionId = solution.Id;
            SolutionName = solution.Name;
            Channel = sla.Channel;
            ContactInformation = sla.ContactInformation;
            From = sla.TimeFrom;
            Until = sla.TimeUntil;
            CanDelete = canDelete;
        }

        public EditSLAContactModel(CatalogueItem solution, SlaContact sla)
        {
            ContactId = sla.Id;
            SolutionId = solution.Id;
            SolutionName = solution.Name;
        }

        public int? ContactId { get; set; }

        public CatalogueItemId SolutionId { get; set; }

        public string Channel { get; set; }

        public string ContactInformation { get; set; }

        [ModelBinder(BinderType = typeof(TimeInputModelBinder))]
        public DateTime From { get; set; }

        [ModelBinder(BinderType = typeof(TimeInputModelBinder))]
        public DateTime Until { get; set; }

        public string SolutionName { get; set; }

        public bool CanDelete { get; set; }
    }
}
