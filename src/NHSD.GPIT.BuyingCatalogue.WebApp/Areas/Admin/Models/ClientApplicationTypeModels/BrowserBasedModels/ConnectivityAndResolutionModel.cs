using System;
using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Constants;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ApplicationTypeModels.BrowserBasedModels
{
    public sealed class ConnectivityAndResolutionModel : ApplicationTypeBaseModel
    {
        public ConnectivityAndResolutionModel()
        {
        }

        public ConnectivityAndResolutionModel(CatalogueItem catalogueItem)
            : base(catalogueItem)
        {
            if (catalogueItem is null)
                throw new ArgumentNullException(nameof(catalogueItem));

            SelectedConnectionSpeed = ClientApplication?.MinimumConnectionSpeed;

            SelectedScreenResolution = ClientApplication?.MinimumDesktopResolution;

            ConnectionSpeeds = SelectLists.ConnectionSpeeds;

            ScreenResolutions = SelectLists.ScreenResolutions;
        }

        public string SelectedConnectionSpeed { get; set; }

        public IReadOnlyList<SelectOption<string>> ConnectionSpeeds { get; set; }

        public string SelectedScreenResolution { get; set; }

        public IReadOnlyList<SelectOption<string>> ScreenResolutions { get; set; }
    }
}
