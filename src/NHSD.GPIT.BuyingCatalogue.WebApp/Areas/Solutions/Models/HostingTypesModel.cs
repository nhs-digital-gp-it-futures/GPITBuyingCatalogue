using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public class HostingTypesModel : SolutionDisplayBaseModel
    {
        public HostingTypesModel(CatalogueItem item)
            : base(item)
        {
            var hosting = item.Solution?.Hosting ?? new Hosting();
            HybridHostingType = hosting.HybridHostingType;
            OnPremise = hosting.OnPremise;
            PrivateCloud = hosting.PrivateCloud;
            PublicCloud = hosting.PublicCloud;
        }

        public HybridHostingType HybridHostingType { get; }

        public OnPremise OnPremise { get; }

        public PrivateCloud PrivateCloud { get; }

        public PublicCloud PublicCloud { get; }

        public override int Index => 10;
    }
}
