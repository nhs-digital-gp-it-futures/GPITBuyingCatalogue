using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        public List<HostingType> HostingTypesToAdd { get; set; }

        [Required(ErrorMessage = "Select a hosting type")]
        public string SelectedHostingType { get; set; }

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
