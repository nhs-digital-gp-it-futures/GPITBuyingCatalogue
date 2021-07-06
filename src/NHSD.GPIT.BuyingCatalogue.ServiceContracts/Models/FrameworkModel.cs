namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models
{
    public class FrameworkModel
    {
        public bool DfocvcFramework { get; set; }

        public bool FoundationSolutionFramework { get; set; }

        public bool GpitFuturesFramework { get; set; }

        public virtual bool IsValid() => DfocvcFramework || GpitFuturesFramework;
    }
}
