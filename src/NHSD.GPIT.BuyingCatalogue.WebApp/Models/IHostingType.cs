using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models;

public interface IHostingType
{
    public HybridHostingType HybridHostingType { get; }

    public OnPremise OnPremise { get; }

    public PrivateCloud PrivateCloud { get; }

    public PublicCloud PublicCloud { get; }
}
