namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models
{
    public sealed class CapabilityEpic
    {
        public string EpicId { get; set; }

        public int CapabilityId { get; set; }

        public CompliancyLevel CompliancyLevel { get; set; }

        public Epic Epic { get; set; }
    }
}
