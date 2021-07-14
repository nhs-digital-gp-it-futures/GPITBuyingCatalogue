using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models
{
    public sealed class ManageCatalogueSolutionModel : NavBaseModel
    {
        public CatalogueItem Solution { get; set; }

        public string LastUpdatedByName { get; set; }

        public PublicationStatus SelectedOption { get; set; }

        public IList<PublicationStatus> PublicationStatuses { get; }
            = Enum.GetValues<PublicationStatus>().ToList();

        public string StatusFeatures() => new FeaturesModel().FromCatalogueItem(Solution).StatusFeatures();
    }
}
