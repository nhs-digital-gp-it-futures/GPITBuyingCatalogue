using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.Components.NhsCareCard.NhsCareCardViewComponent;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.Components.NhsCareCard
{
    public sealed class NhsCareCardModel
    {
        public string Title { get; set; }

        public List<string> ListOptions { get; set; }

        public string CareCardClass { get; set; }

        public string Footer { get; set; }
    }
}
