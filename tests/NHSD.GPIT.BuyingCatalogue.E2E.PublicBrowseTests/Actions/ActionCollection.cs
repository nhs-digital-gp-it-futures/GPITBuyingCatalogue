using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHSD.GPIT.BuyingCatalogue.E2E.PublicBrowseTests.Actions
{
    internal sealed class ActionCollection
    {
        internal SolutionsActions SolutionsActions { get; set; }
        internal BuyersGuideActions BuyersGuideActions { get; set; }
        internal CommonActions CommonActions { get; set; }
        internal HomepageActions HomePageActions { get; set; }
    }
}
