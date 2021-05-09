﻿using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.ClientApplicationType
{
    public class BrowserBasedModel : MarketingBaseModel
    { 
        public BrowserBasedModel() : base(null)
        {
            ClientApplication = new ClientApplication();
        }

        public BrowserBasedModel(CatalogueItem catalogueItem) : base(catalogueItem)
        {
            if (catalogueItem is null)
                throw new ArgumentNullException(nameof(catalogueItem));

            BackLink = $"/marketing/supplier/solution/{CatalogueItem.CatalogueItemId}";
        }

        public override bool? IsComplete =>
            ClientApplication == null ? false : ClientApplication.BrowserBasedModelComplete();

        public string SupportedBrowsersStatus => (ClientApplication?.SupportedBrowsersComplete()).ToStatus();

        public string MobileFirstApproachStatus => (ClientApplication?.NativeMobileFirstApproachComplete()).ToStatus();

        public string PlugInsStatus => (ClientApplication?.PlugInsComplete()).ToStatus();

        public string ConnectivityStatus => (ClientApplication?.ConnectivityAndResolutionComplete()).ToStatus();

        public string HardwareRequirementsStatus => (ClientApplication?.HardwareRequirementsComplete()).ToStatus();

        public string AdditionalInformationStatus => (ClientApplication?.AdditionalInformationComplete()).ToStatus();
    }
}
