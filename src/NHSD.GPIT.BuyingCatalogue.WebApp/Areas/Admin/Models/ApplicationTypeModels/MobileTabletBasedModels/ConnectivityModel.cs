using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ApplicationTypeModels.MobileTabletBasedModels
{
    public sealed class ConnectivityModel : ApplicationTypeBaseModel
    {
        public ConnectivityModel()
        {
            ConnectionSpeeds = Framework.Constants.SelectLists.ConnectionSpeeds;
            SetConnectionTypes();
        }

        public ConnectivityModel(CatalogueItem catalogueItem)
            : base(catalogueItem)
        {
            if (catalogueItem is null)
                throw new ArgumentNullException(nameof(catalogueItem));

            ConnectionSpeeds = Framework.Constants.SelectLists.ConnectionSpeeds;
            SetConnectionTypes();

            SelectedConnectionSpeed = ApplicationTypeDetail?.MobileConnectionDetails?.MinimumConnectionSpeed;

            CheckConnectionTypes();

            Description = ApplicationTypeDetail?.MobileConnectionDetails?.Description;
        }

        public string SelectedConnectionSpeed { get; set; }

        public IReadOnlyList<SelectOption<string>> ConnectionSpeeds { get; set; }

        public ConnectionTypeModel[] ConnectionTypes { get; set; }

        [StringLength(300)]
        public string Description { get; set; }

        private void CheckConnectionTypes()
        {
            foreach (var connectionType in ConnectionTypes)
            {
                if (ApplicationTypeDetail?.MobileConnectionDetails?.ConnectionType != null &&
                    ApplicationTypeDetail.MobileConnectionDetails.ConnectionType.Any(s => s.Equals(connectionType.ConnectionType, StringComparison.InvariantCultureIgnoreCase)))
                {
                    connectionType.Checked = true;
                }
            }
        }

        private void SetConnectionTypes()
        {
            ConnectionTypes = new ConnectionTypeModel[]
            {
                new() { ConnectionType = "GPRS" },
                new() { ConnectionType = "3G" },
                new() { ConnectionType = "LTE" },
                new() { ConnectionType = "4G" },
                new() { ConnectionType = "5G" },
                new() { ConnectionType = "Bluetooth" },
                new() { ConnectionType = "Wifi" },
            };
        }
    }
}
