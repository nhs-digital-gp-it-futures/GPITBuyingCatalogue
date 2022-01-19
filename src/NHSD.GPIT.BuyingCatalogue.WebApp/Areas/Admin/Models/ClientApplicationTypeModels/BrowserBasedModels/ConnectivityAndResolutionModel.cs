using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Constants;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ClientApplicationTypeModels.BrowserBasedModels
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

        public IReadOnlyList<SelectListItem> ConnectionSpeeds { get; set; }

        public string SelectedScreenResolution { get; set; }

        public IReadOnlyList<SelectListItem> ScreenResolutions { get; set; }
    }
}
