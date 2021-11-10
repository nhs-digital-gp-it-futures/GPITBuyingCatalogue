using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.DesktopBasedModels
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

            BackLink = $"/admin/catalogue-solutions/manage/{catalogueItem.Id}/client-application-type/desktop";

            ConnectionSpeeds = Framework.Constants.SelectLists.ConnectionSpeeds;

            SelectedConnectionSpeed = ClientApplication?.NativeDesktopMinimumConnectionSpeed;
        }

        [Required(ErrorMessage = "Select a connection speed")]
        public string SelectedConnectionSpeed { get; set; }

        public IReadOnlyList<SelectListItem> ConnectionSpeeds { get; init; }
    }
}
