using System;
using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models
{
    public class Contract
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public int? ImplementationPlanId { get; set; }

        public ImplementationPlan ImplementationPlan { get; set; }

        public virtual Order Order { get; set; }
    }
}
