namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions
{
    public sealed class NativeDesktopThirdParty
    {
        public string DeviceCapabilities { get; set; }

        public string ThirdPartyComponents { get; set; }

        public bool IsValid() =>
            !string.IsNullOrWhiteSpace(DeviceCapabilities) ||
            !string.IsNullOrWhiteSpace(ThirdPartyComponents);
    }
}
