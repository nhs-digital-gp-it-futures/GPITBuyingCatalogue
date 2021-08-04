namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions
{
    public sealed class MobileMemoryAndStorage
    {
        public string Description { get; set; }

        public string MinimumMemoryRequirement { get; set; }

        public bool IsValid() =>
            !string.IsNullOrWhiteSpace(Description) &&
            !string.IsNullOrWhiteSpace(MinimumMemoryRequirement);
    }
}
