using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ApplicationTypeModels.DesktopBasedModels
{
    public sealed class DesktopBasedModel : ApplicationTypeBaseModel
    {
        public DesktopBasedModel()
        {
        }

        public DesktopBasedModel(CatalogueItem catalogueItem)
            : base(catalogueItem)
        {
            ArgumentNullException.ThrowIfNull(catalogueItem);

            ApplicationType = ApplicationType.Desktop;
        }
    }
}
