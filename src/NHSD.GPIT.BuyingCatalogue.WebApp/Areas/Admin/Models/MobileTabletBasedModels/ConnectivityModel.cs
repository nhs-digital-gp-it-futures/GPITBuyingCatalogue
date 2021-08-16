﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.MobileTabletBasedModels
{
    public class ConnectivityModel : ApplicationTypeBaseModel
    {
        public ConnectivityModel()
            : base()
        {
            ConnectionSpeeds = Framework.Constants.SelectLists.ConnectionSpeeds;
            SetConnectionTypes();
        }

        public ConnectivityModel(CatalogueItem catalogueItem)
            : base(catalogueItem)
        {
            if (catalogueItem is null)
                throw new ArgumentNullException(nameof(catalogueItem));

            BackLink = $"/admin/catalogue-solutions/manage/{catalogueItem.Id}/client-application-type/mobiletablet";

            ConnectionSpeeds = Framework.Constants.SelectLists.ConnectionSpeeds;
            SetConnectionTypes();

            SelectedConnectionSpeed = ClientApplication?.MobileConnectionDetails?.MinimumConnectionSpeed;

            CheckConnectionTypes();

            Description = ClientApplication?.MobileConnectionDetails?.Description;
        }

        public override bool IsComplete =>
            !string.IsNullOrWhiteSpace(SelectedConnectionSpeed) ||
            ConnectionTypes.Any();

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
                    ClientApplication.MobileConnectionDetails.ConnectionType.Any(s => s.Equals(connectionType.ConnectionType, StringComparison.InvariantCultureIgnoreCase)))
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
