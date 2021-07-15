using NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Admin.EditSolution;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Admin
{
    internal sealed class ActionCollection
    {
        internal CommonActions CommonActions { get; set; }

        internal UserDetails UserDetails { get; set; }

        internal Organisation Organisation { get; set; }

        internal Dashboard Dashboard { get; set; }

        internal AddUser AddUser { get; set; }

        internal AddRelatedOrganisation AddRelatedOrganisation { get; set; }

        internal AddSolution AddSolution { get; set; }

        internal Features Features { get; set; }
    }
}
