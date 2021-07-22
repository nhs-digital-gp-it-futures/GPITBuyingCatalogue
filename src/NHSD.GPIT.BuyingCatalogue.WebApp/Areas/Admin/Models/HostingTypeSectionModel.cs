using System;
using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.HostingTypeModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models
{
    public class HostingTypeSectionModel : MarketingBaseModel
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

            var hosting = catalogueItem.Solution?.GetHosting();

            SolutionName = catalogueItem?.Name;

            PublicCloud = new PublicCloudModel(hosting?.PublicCloud);

            PrivateCloud = new PrivateCloudModel(hosting?.PrivateCloud);

            Hybrid = new HybridModel(hosting?.HybridHostingType);

            OnPremise = new OnPremiseModel(hosting?.OnPremise);

            PopulateHostingTypesToAdd();
        }

        public PublicCloudModel PublicCloud { get; set; }

        public PrivateCloudModel PrivateCloud { get; set; }

        public HybridModel Hybrid { get; set; }

        public OnPremiseModel OnPremise { get; set; }

        public string SolutionName { get; set; }

        public List<HostingType> HostingTypesToAdd { get; set; }

        public override bool? IsComplete =>
            Convert.ToBoolean(PublicCloud?.IsComplete) ||
            Convert.ToBoolean(PrivateCloud?.IsComplete) ||
            Convert.ToBoolean(Hybrid?.IsComplete) ||
            Convert.ToBoolean(OnPremise?.IsComplete);

        public bool? AddedHostingType => IsComplete == true ? true : null;

        public FeatureCompletionStatus StatusHostingType() =>
           Convert.ToBoolean(IsComplete)
               ? FeatureCompletionStatus.Completed
               : FeatureCompletionStatus.NotStarted;

        private void PopulateHostingTypesToAdd()
        {
            HostingTypesToAdd = new List<HostingType>();

            if (!Convert.ToBoolean(PublicCloud?.IsValid()))
                HostingTypesToAdd.Add(new HostingType("Public cloud", "PublicCloud"));

            if (!Convert.ToBoolean(PrivateCloud?.IsValid()))
                HostingTypesToAdd.Add(new HostingType("Private cloud", "PrivateCloud"));

            if (!Convert.ToBoolean(Hybrid?.IsValid()))
                HostingTypesToAdd.Add(new HostingType("Hybrid", "HybridCloud"));

            if (!Convert.ToBoolean(OnPremise?.IsValid()))
                HostingTypesToAdd.Add(new HostingType("On premise", "OnPremiseCloud"));
        }
    }
}
