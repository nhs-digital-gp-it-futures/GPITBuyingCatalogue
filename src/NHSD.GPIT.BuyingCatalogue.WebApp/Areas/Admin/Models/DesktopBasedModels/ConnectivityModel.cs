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

            BackLinkText = "Go back";
        }

        public ConnectivityModel(CatalogueItem catalogueItem)
            : base(catalogueItem)
        {
            if (catalogueItem is null)
                throw new ArgumentNullException(nameof(catalogueItem));

            BackLink = $"/admin/catalogue-solutions/manage/{catalogueItem.Id}/client-application-type/desktop";

            ConnectionSpeeds = Framework.Constants.SelectLists.ConnectionSpeeds;

            SelectedConnectionSpeed = ClientApplication?.NativeDesktopMinimumConnectionSpeed;

            BackLinkText = "Go back";
        }

        public override bool IsComplete => !string.IsNullOrWhiteSpace(SelectedConnectionSpeed);

        [Required(ErrorMessage = "Select a connection speed")]
        public string SelectedConnectionSpeed { get; set; }

        public List<SelectListItem> ConnectionSpeeds { get; set; }
    }
}
