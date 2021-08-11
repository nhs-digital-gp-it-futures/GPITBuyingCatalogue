namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models
{
    public sealed class Epic
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public int CapabilityId { get; set; }

        public string SourceUrl { get; set; }

        public bool Active { get; set; }

        public bool SupplierDefined { get; set; }

        public CompliancyLevel CompliancyLevel { get; set; }
    }
}
