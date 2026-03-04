using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ApplicationTypeModels.MobileTabletBasedModels
{
    public sealed class MobileTabletBasedModel : ApplicationTypeBaseModel
    {
        public MobileTabletBasedModel()
        {
        }

        public MobileTabletBasedModel(CatalogueItem catalogueItem)
            : base(catalogueItem)
        {
            ArgumentNullException.ThrowIfNull(catalogueItem);

            ApplicationType = ApplicationType.MobileTablet;
        }
    }
}
