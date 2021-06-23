using System.Collections.Generic;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models
{
    public class CatalogueSolutionsModel
    {
        public List<CatalogueModel> CatalogueItems { get; set; }

        public IList<PublicationStatusModel> AllPublicationStatuses { get; set; }

        public int PublicationStatusId { get; set; }

        // public string GetStatusClass(int statusId)
        // {
        //     string colour = "white";
        //
        //     if (PublicationStatus.Published.Id == statusId)
        //         colour = "white";
        //     if (PublicationStatus.Unpublished.Id == statusId)
        //         colour = "yellow";
        //     if (PublicationStatus.Published.Id == statusId)
        //         colour = "green";
        //     if (PublicationStatus.Withdrawn.Id == statusId)
        //         colour = "red";
        //
        //     return $"nhsuk-tag--{colour}";
        // }
    }
}
