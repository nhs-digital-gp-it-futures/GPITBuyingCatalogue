using System.Diagnostics.CodeAnalysis;

namespace NHSD.GPIT.BuyingCatalogue.FinalMigration
{
    [ExcludeFromCodeCoverage]
    sealed class Reconciliation : BaseMigrator
    {
        public Reconciliation() : base()
        {
        }

        public void RunReconciliation()
        {
            System.Diagnostics.Trace.WriteLine("####### STARTING RECONCILIATION #######");

            LoadLegacyData();
        }
    }
}
