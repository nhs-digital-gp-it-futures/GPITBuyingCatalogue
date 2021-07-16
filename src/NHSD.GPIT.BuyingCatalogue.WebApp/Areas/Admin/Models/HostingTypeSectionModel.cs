using System;
using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models
{
    public sealed class HostingTypeSectionModel : MarketingBaseModel
    {
        public HostingTypeSectionModel()
            : base(null)
        {
        }

        public HostingTypeSectionModel(CatalogueItem catalogueItem)
            : base(catalogueItem)
        {
            if (catalogueItem is null)
                throw new ArgumentNullException(nameof(catalogueItem));

            SolutionName = catalogueItem?.Name;
            PublicCloud = CatalogueItem.Solution?.GetHosting()?.PublicCloud;
            PrivateCloud = CatalogueItem.Solution?.GetHosting()?.PrivateCloud;
            Hybrid = CatalogueItem.Solution?.GetHosting()?.HybridHostingType;
            OnPremise = CatalogueItem.Solution?.GetHosting()?.OnPremise;
            PopulateHostingTypesToAdd();
        }

        public PublicCloud PublicCloud { get; set; }

        public PrivateCloud PrivateCloud { get; set; }

        public HybridHostingType Hybrid { get; set; }

        public OnPremise OnPremise { get; set; }

        public string SolutionName { get; set; }

        public Dictionary<string, string> HostingTypesToAdd { get; set; }

        public override bool? IsComplete =>
            Convert.ToBoolean(PublicCloud?.IsValid()) ||
            Convert.ToBoolean(PrivateCloud?.IsValid()) ||
            Convert.ToBoolean(Hybrid?.IsValid()) ||
            Convert.ToBoolean(OnPremise?.IsValid());

        public FeatureCompletionStatus StatusHostingType() =>
           Convert.ToBoolean(IsComplete)
               ? FeatureCompletionStatus.Completed
               : FeatureCompletionStatus.NotStarted;

        private void PopulateHostingTypesToAdd()
        {
            HostingTypesToAdd = new Dictionary<string, string>();

            if (Convert.ToBoolean(PublicCloud?.IsValid()))
                HostingTypesToAdd.Add("PublicCloud", "Public cloud");

            if (Convert.ToBoolean(PrivateCloud?.IsValid()))
                HostingTypesToAdd.Add("PrivateCloud", "Private cloud");

            if (Convert.ToBoolean(Hybrid?.IsValid()))
                HostingTypesToAdd.Add("HybridCloud", "Hybrid cloud");

            if (Convert.ToBoolean(OnPremise?.IsValid()))
                HostingTypesToAdd.Add("OnPremiseCloud", "On premise");
        }
    }
}
