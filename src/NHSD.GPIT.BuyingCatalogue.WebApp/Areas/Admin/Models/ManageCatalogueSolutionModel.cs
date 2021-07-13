using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models
{
    public sealed class ManageCatalogueSolutionModel : NavBaseModel
    {
        public ManageCatalogueSolutionModel()
        {
        }

        public CatalogueItem Solution { get; set; }

        public string LastUpdatedByName { get; set; }

        public EntityFramework.Catalogue.Models.PublicationStatus SelectedOption { get; set; }

        public IList<EntityFramework.Catalogue.Models.PublicationStatus> PublicationStatuses { get; }
            = Enum.GetValues<EntityFramework.Catalogue.Models.PublicationStatus>().ToList();
    }
}
