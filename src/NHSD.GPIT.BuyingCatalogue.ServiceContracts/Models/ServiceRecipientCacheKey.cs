namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;

public readonly struct ServiceRecipientCacheKey
{
    private readonly object[] keys;

    public ServiceRecipientCacheKey(
        params object[] keys)
    {
        this.keys = keys;
    }

    public override string ToString()
        => string.Join('-', keys);
}
