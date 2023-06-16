using System;
using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ApplicationTypeModels.DesktopBasedModels
{
    public sealed class ConnectivityModel : ApplicationTypeBaseModel
    {
        public ConnectivityModel()
        {
            ConnectionSpeeds = Framework.Constants.SelectLists.ConnectionSpeeds;
        }

        public ConnectivityModel(CatalogueItem catalogueItem)
            : base(catalogueItem)
        {
            if (catalogueItem is null)
                throw new ArgumentNullException(nameof(catalogueItem));

            ConnectionSpeeds = Framework.Constants.SelectLists.ConnectionSpeeds;

            SelectedConnectionSpeed = ApplicationTypes?.NativeDesktopMinimumConnectionSpeed;
        }

        public string SelectedConnectionSpeed { get; set; }

        public IReadOnlyList<SelectOption<string>> ConnectionSpeeds { get; init; }
    }
}
