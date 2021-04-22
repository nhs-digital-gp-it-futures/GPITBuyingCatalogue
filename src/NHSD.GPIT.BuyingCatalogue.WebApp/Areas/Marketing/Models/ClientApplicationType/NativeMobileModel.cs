﻿using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.ClientApplicationType
{
    public class NativeMobileModel : MarketingBaseModel
    { 
        public NativeMobileModel(CatalogueItem catalogueItem) : base(catalogueItem)
        {
            BackLink = $"/marketing/supplier/solution/{CatalogueItem.CatalogueItemId}";                 
        }

        protected override bool IsComplete
        {
            get { throw new NotImplementedException(); }
        }                

        public string SupportedOperatingSystemsStatus
        {
            get { return "TODO"; }
        }

        public string MobileFirstApproachStatus
        {
            get { return "TODO"; }
        }

        public string ConnectivityStatus
        {
            get { return "TODO"; }
        }

        public string MemoryAndStorageStatus
        {
            get { return "TODO"; }
        }

        public string ThirdPartyStatus
        {
            get { return "TODO"; }
        }

        public string HardwareRequirementsStatus
        {
            get { return "TODO"; }
        }

        public string AdditionalInformationStatus
        {
            get { return "TODO"; }
        }
    }
}
