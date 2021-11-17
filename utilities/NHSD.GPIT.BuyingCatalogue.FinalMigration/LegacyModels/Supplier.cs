using System;
using System.Collections.Generic;

#nullable disable

namespace NHSD.GPIT.BuyingCatalogue.FinalMigration.LegacyModels
{
    public partial class Supplier
    {       
        public string Id { get; set; }
        public string Name { get; set; }
        public int AddressId { get; set; }
    }
}
