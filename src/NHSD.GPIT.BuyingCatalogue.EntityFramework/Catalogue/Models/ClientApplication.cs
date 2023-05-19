using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using EnumsNET;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models.Serialization;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models
{
    public sealed class ClientApplication
    {
        public HashSet<string> ClientApplicationTypes { get; set; } = new();

        [JsonConverter(typeof(SupportedBrowsersJsonConverter))]
        public HashSet<SupportedBrowser> BrowsersSupported { get; set; } = new();

        public bool? MobileResponsive { get; set; }

        public Plugins Plugins { get; set; }

        public string HardwareRequirements { get; set; }

        public string NativeMobileHardwareRequirements { get; set; }

        public string NativeDesktopHardwareRequirements { get; set; }

        public string AdditionalInformation { get; set; }

        public string MinimumConnectionSpeed { get; set; }

        public string MinimumDesktopResolution { get; set; }

        public bool? MobileFirstDesign { get; set; }

        public bool? NativeMobileFirstDesign { get; set; }

        public MobileOperatingSystems MobileOperatingSystems { get; set; }

        public MobileConnectionDetails MobileConnectionDetails { get; set; }

        public MobileMemoryAndStorage MobileMemoryAndStorage { get; set; }

        public MobileThirdParty MobileThirdParty { get; set; }

        public string NativeMobileAdditionalInformation { get; set; }

        public string NativeDesktopOperatingSystemsDescription { get; set; }

        public string NativeDesktopMinimumConnectionSpeed { get; set; }

        public NativeDesktopThirdParty NativeDesktopThirdParty { get; set; }

        public NativeDesktopMemoryAndStorage NativeDesktopMemoryAndStorage { get; set; }

        public string NativeDesktopAdditionalInformation { get; set; }

        public IReadOnlyList<ClientApplicationType> ExistingClientApplicationTypes
        {
            get
            {
                var result = new List<ClientApplicationType>(3);

                foreach (ClientApplicationType clientApplicationType in Enum.GetValues(typeof(ClientApplicationType)))
                {
                    if (ClientApplicationTypes?.Any(type => type.Equals(clientApplicationType.AsString(EnumFormat.EnumMemberValue), StringComparison.OrdinalIgnoreCase)) ?? false)
                        result.Add(clientApplicationType);
                }

                return result;
            }
        }

        public void EnsureClientApplicationTypePresent(ClientApplicationType clientApplicationType)
        {
            ClientApplicationTypes ??= new HashSet<string>();

            if (!ClientApplicationTypes.Any(type => type.Equals(clientApplicationType.AsString(EnumFormat.EnumMemberValue), StringComparison.OrdinalIgnoreCase)))
                ClientApplicationTypes.Add(clientApplicationType.AsString(EnumFormat.EnumMemberValue));
        }

        public bool HasClientApplicationType(ClientApplicationType clientApplicationType)
            => ClientApplicationTypes?.Any(type => type.Equals(clientApplicationType.AsString(EnumFormat.EnumMemberValue), StringComparison.OrdinalIgnoreCase)) ?? false;
    }
}
