using System;
using System.Collections.Generic;

#nullable disable

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue
{
    public partial class MarketingContact
    {
        public int Id { get; set; }
        public string SolutionId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Department { get; set; }
        public DateTime LastUpdated { get; set; }
        public Guid LastUpdatedBy { get; set; }

        public virtual Solution Solution { get; set; }
    }
}
