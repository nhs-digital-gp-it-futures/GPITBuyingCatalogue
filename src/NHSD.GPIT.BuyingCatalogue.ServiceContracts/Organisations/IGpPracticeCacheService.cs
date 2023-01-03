namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations
{
    public interface IGpPracticeCacheService
    {
        int? GetNumberOfPatients(string odsCode);

        void Refresh();
    }
}
