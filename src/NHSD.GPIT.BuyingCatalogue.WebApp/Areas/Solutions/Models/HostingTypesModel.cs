using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public class HostingTypesModel : SolutionDisplayBaseModel, IHostingType
    {
        public HostingTypesModel(CatalogueItem item, CatalogueItemContentStatus contentStatus)
            : base(item, contentStatus)
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
