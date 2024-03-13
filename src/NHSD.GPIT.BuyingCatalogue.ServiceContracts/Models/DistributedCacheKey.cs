namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;

public readonly struct DistributedCacheKey(params object[] keys)
{
    public override string ToString()
        => string.Join('-', keys);
}
