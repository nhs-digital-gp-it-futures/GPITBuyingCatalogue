using System;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.WebApp.Models
{
    public sealed class TimeInputModel
    {
        public DateTime? From { get; set; }

        public DateTime? Until { get; set; }

        public DateTime? SingleFrom { get; set; }
    }
}
