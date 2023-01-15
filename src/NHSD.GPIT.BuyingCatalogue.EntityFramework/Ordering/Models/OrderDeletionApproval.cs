using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models
{
    [Serializable]
    public class OrderDeletionApproval : IAudited
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public string NameOfRequester { get; set; }

        public string NameOfApprover { get; set; }

        public DateTime DateOfApproval { get; set; }

        public DateTime LastUpdated { get; set; }

        public int? LastUpdatedBy { get; set; }

        public AspNetUser LastUpdatedByUser { get; set; }

        public virtual Order Order { get; set; }
    }
}
