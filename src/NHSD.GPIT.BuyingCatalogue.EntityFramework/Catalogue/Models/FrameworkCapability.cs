namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models
{
    public sealed class FrameworkCapability
    {
        public string FrameworkId { get; set; }

        public int CapabilityId { get; set; }

        public bool IsFoundation { get; set; }

        public Capability Capability { get; set; }

        public Framework Framework { get; set; }
    }
}
