namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.FilterModels
{
    public sealed class HostFilterInfo
    {
        public string Id { get; init; }

        public string ShortName { get; init; }

        public int CountOfActiveSolutions { get; set; }
    }
}
