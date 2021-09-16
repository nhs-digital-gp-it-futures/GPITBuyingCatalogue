using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions
{
    public sealed class NativeDesktopThirdParty
    {
        public string DeviceCapabilities { get; set; }

        public string ThirdPartyComponents { get; set; }

        public TaskProgress Status()
        {
            if (!string.IsNullOrWhiteSpace(DeviceCapabilities) || !string.IsNullOrWhiteSpace(ThirdPartyComponents))
                return TaskProgress.Completed;

            return TaskProgress.NotStarted;
        }
    }
}
