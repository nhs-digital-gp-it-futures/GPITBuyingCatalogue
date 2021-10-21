namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models
{
    public sealed class Hosting
    {
        public PublicCloud PublicCloud { get; set; } = new();

        public PrivateCloud PrivateCloud { get; set; } = new();

        public HybridHostingType HybridHostingType { get; set; } = new();

        public OnPremise OnPremise { get; set; } = new();
    }
}
