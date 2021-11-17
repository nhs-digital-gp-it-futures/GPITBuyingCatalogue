using System;
using System.Collections.Generic;

#nullable disable

namespace NHSD.GPIT.BuyingCatalogue.FinalMigration.LegacyModels
{
    public partial class CatalogueItem
    {
        public string Id { get; set; }
        public int CatalogueItemTypeId { get; set; }
        public string Name { get; set; }
        public string ParentCatalogueItemId { get; set; }
    }
}
