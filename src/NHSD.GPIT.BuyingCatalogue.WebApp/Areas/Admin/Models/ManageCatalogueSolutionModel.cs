using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;
using static NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers.Tags.NhsTagsTagHelper;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models
{
    public sealed class ManageCatalogueSolutionModel : NavBaseModel
    {
        public CatalogueItem Solution { get; set; }

        public string LastUpdatedByName { get; set; }

        public PublicationStatus SelectedOption { get; set; }

        public IList<PublicationStatus> PublicationStatuses { get; }
            = Enum.GetValues<PublicationStatus>().ToList();

        public string StatusDescription() => new DescriptionModel(Solution).StatusDescription();

        public TagColour StatusDescriptionColor() => new DescriptionModel(Solution).StatusDescriptionColor();

        public string StatusFeatures() => new FeaturesModel().FromCatalogueItem(Solution).StatusFeatures();

        public TagColour StatusFeaturesColor() => new FeaturesModel().FromCatalogueItem(Solution).StatusFeaturesColor();

        public string StatusImplementation() => new ImplementationTimescaleModel(Solution).StatusImplementation();

        public TagColour StatusImplementationColor() => new ImplementationTimescaleModel(Solution).StatusImplementationColor();
    }
}
