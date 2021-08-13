namespace NHSD.GPIT.BuyingCatalogue.EntityFramework
{
    public interface IAudited
    {
        void SetLastUpdatedBy(int userId, string userName);
    }
}
