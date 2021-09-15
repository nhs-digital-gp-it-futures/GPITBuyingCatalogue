using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions
{
    public sealed class MobileThirdParty
    {
        public string ThirdPartyComponents { get; set; }

        public string DeviceCapabilities { get; set; }

        public TaskProgress Status()
        {
            if (!string.IsNullOrWhiteSpace(ThirdPartyComponents) ||
                !string.IsNullOrWhiteSpace(DeviceCapabilities))
            {
                return TaskProgress.Completed;
            }

            return TaskProgress.NotStarted;
        }
    }
}
