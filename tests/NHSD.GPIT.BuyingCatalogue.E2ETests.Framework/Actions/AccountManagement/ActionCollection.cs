
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common.Organisation;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.AccountManagement
{
    public sealed class ActionCollection
    {
        public CommonActions CommonActions { get; set; }

        public AddUser AddUser { get; set; }

        public Details Details { get; set; }
    }

}
