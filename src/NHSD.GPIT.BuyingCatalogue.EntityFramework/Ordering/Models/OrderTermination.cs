using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models
{
    [Serializable]
    public class OrderTermination
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public string Reason { get; set; }

        public DateTime DateOfTermination { get; set; }

        public virtual Order Order { get; set; }
    }
}
