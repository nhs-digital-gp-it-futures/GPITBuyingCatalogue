namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models
{
    public sealed class StandardCapability
    {
        public string StandardId { get; set; }

        public int CapabilityId { get; set; }

        public Standard Standard { get; set; }

        public Capability Capability { get; set; }
    }
}
