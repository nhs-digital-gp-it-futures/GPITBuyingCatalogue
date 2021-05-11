using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.NativeMobile
{
    public class ConnectivityModel : MarketingBaseModel
    {
        public ConnectivityModel() : base(null)
        {
        }

        public ConnectivityModel(CatalogueItem catalogueItem) : base(catalogueItem)
        {
            if (catalogueItem is null)
                throw new ArgumentNullException(nameof(catalogueItem));

            BackLink = $"/marketing/supplier/solution/{CatalogueItem.CatalogueItemId}/section/native-mobile";

            SelectedConnectionSpeed = ClientApplication?.MobileConnectionDetails?.MinimumConnectionSpeed;

            CheckConnectionTypes();

            Description = ClientApplication?.MobileConnectionDetails?.Description;            
        }

        public override bool? IsComplete
        {
            get 
            {
                if (!string.IsNullOrWhiteSpace(ClientApplication?.MobileConnectionDetails?.MinimumConnectionSpeed) ||
                  !string.IsNullOrWhiteSpace(ClientApplication?.MobileConnectionDetails?.Description))
                    return true;

                return ClientApplication?.MobileConnectionDetails?.ConnectionType?.Any();
            }
        }

        public string SelectedConnectionSpeed { get; set; }
        public List<SelectListItem> ConnectionSpeeds { get; set; }

        public ConnectionTypeModel[] ConnectionTypes { get; set; }

        [StringLength(300)]
        public string Description { get; set; }

        private void CheckConnectionTypes()
        {
            foreach (var connectionType in ConnectionTypes)
            {
                if (ClientApplication?.MobileConnectionDetails?.ConnectionType != null &&
                    ClientApplication.MobileConnectionDetails.ConnectionType.Any(x => x.Equals(connectionType.ConnectionType, StringComparison.InvariantCultureIgnoreCase)))
                {
                    connectionType.Checked = true;
                }
            }
        }
    }
}
