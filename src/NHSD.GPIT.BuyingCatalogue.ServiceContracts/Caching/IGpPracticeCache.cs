namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Caching
{
    public interface IGpPracticeCache
    {
        int? Get(string odsCode);

        void Set(string odsCode, int numberOfPatients);

        void RemoveAll();
    }
}
