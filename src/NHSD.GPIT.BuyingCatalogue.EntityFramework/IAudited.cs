using System;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework
{
    public interface IAudited
    {
        void SetLastUpdatedBy(Guid userId, string userName);
    }
}
