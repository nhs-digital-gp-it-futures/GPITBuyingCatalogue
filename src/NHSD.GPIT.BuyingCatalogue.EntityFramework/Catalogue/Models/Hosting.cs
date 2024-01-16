using System;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models
{
    [Serializable]
    public sealed class Hosting
    {
        public PublicCloud PublicCloud { get; set; } = new();

        public PrivateCloud PrivateCloud { get; set; } = new();

        public HybridHostingType HybridHostingType { get; set; } = new();

        public OnPremise OnPremise { get; set; } = new();

        public bool IsValid()
            => (PublicCloud?.IsValid() ?? false)
               || (PrivateCloud?.IsValid() ?? false)
               || (HybridHostingType?.IsValid() ?? false)
               || (OnPremise?.IsValid() ?? false);
    }
}
