using System;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models
{
    public class FrameworkModel
    {
        public string Name { get; set; }

        [Obsolete("All solutions are foundation solutions so this will be removed as part of story #23333")]
        public bool IsFoundation { get; set; }

        public string FrameworkId { get; set; }

        public bool Selected { get; set; }
    }
}
