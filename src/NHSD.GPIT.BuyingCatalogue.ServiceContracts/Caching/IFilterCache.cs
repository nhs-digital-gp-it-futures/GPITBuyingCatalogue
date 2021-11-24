namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Caching
{
    public interface IFilterCache
    {
        string Get(string filterKey);

        void Set(string filterKey, string content);

        void Remove(string filterKey);

        void RemoveAll();
    }
}
