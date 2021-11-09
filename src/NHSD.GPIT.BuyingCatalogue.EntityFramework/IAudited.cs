using System;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework
{
    public interface IAudited
    {
        int? LastUpdatedBy { get; set; }

        DateTime LastUpdated { get; set; }
    }
}
