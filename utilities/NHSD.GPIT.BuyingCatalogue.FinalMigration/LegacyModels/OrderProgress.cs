using System;
using System.Collections.Generic;

#nullable disable

namespace NHSD.GPIT.BuyingCatalogue.FinalMigration.LegacyModels
{
    public partial class OrderProgress
    {
        public int OrderId { get; set; }
        public bool CatalogueSolutionsViewed { get; set; }
        public bool AdditionalServicesViewed { get; set; }
        public bool AssociatedServicesViewed { get; set; }        
    }
}
