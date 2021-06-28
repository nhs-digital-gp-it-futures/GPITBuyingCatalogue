using System.Collections.Generic;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models
{
    public class CatalogueSolutionsModel
    {
        public IList<PublicationStatusModel> AllPublicationStatuses { get; set; }

        public List<CatalogueModel> CatalogueItems { get; set; }

        public int PublicationStatusId { get; set; }
    }
}
