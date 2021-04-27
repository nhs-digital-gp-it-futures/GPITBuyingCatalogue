﻿using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.HostingType
{
    public class PrivateCloudModel : MarketingBaseModel
    {
        public PrivateCloudModel() : base(null)
        {
        }

        public PrivateCloudModel(CatalogueItem catalogueItem) : base(catalogueItem)
        {
            if (catalogueItem is null)
                throw new ArgumentNullException(nameof(catalogueItem));

            BackLink = $"/marketing/supplier/solution/{CatalogueItem.CatalogueItemId}";                     
            PrivateCloud = catalogueItem.Solution.GetHosting().PrivateCloud;
        }

        public override bool? IsComplete =>
            !string.IsNullOrWhiteSpace(PrivateCloud?.Summary) ||
            !string.IsNullOrWhiteSpace(PrivateCloud?.Link) ||
            !string.IsNullOrWhiteSpace(PrivateCloud?.RequiresHscn) ||
            !string.IsNullOrWhiteSpace(PrivateCloud?.HostingModel);

        public PrivateCloud PrivateCloud { get; set; }

        public bool RequiresHscnChecked
        {
            get => !string.IsNullOrWhiteSpace(PrivateCloud?.RequiresHscn);
            set => PrivateCloud.RequiresHscn = value ? "End user devices must be connected to HSCN/N3" : null;
        }
    }
}
