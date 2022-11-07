using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Admin.EditSolution;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common.Organisation;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Admin
{
    public sealed class ActionCollection
    {
        public CommonActions CommonActions { get; set; }

        public UserDetails UserDetails { get; set; }

        public Organisation Organisation { get; set; }

        public Dashboard Dashboard { get; set; }

        public AddUser AddUser { get; set; }

        public AddRelatedOrganisation AddRelatedOrganisation { get; set; }

        public AddSolution AddSolution { get; set; }

        public Details Details { get; set; }

        public Features Features { get; set; }
    }
}
