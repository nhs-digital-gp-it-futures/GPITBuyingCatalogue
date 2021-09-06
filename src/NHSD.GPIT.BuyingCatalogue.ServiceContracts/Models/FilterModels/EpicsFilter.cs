namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models
{
    public sealed class EpicsFilter
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public int Count { get; set; }

        public bool Selected { get; set; }

        public string DisplayText => $"{Name} ({Count})";
    }
}
