namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.FilterModels
{
    public sealed class EpicsFilter
    {
        public string Id { get; init; }

        public string Name { get; init; }

        public int Count { get; init; }

        public bool Selected { get; init; }

        public string DisplayText => $"{Name} ({Count})";
    }
}
