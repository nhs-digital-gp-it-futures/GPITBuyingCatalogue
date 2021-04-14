using System;
using System.Collections.Generic;

#nullable disable

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Ordering
{
    public partial class ServiceInstanceItem
    {
        public int OrderId { get; set; }
        public string CatalogueItemId { get; set; }
        public string OdsCode { get; set; }
        public string ServiceInstanceId { get; set; }
    }
}
