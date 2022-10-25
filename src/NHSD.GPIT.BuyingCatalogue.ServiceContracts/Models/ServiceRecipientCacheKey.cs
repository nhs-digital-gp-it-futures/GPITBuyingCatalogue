using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;

public readonly struct ServiceRecipientCacheKey
{
    private readonly int userId;
    private readonly string internalOrgId;
    private readonly CallOffId callOffId;
    private readonly CatalogueItemId catalogueItemId;

    public ServiceRecipientCacheKey(
        int userId,
        string internalOrgId,
        CallOffId callOffId,
        CatalogueItemId catalogueItemId)
    {
        this.userId = userId;
        this.internalOrgId = internalOrgId;
        this.callOffId = callOffId;
        this.catalogueItemId = catalogueItemId;
    }

    public override string ToString()
        => $"{userId}-{internalOrgId}-{callOffId}-{catalogueItemId}";
}
