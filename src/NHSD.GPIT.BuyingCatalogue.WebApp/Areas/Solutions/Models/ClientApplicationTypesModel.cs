using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public sealed class ClientApplicationTypesModel : SolutionDisplayBaseModel
    {
        private const string KeyBrowserBased = "browser-based";
        private const string KeyNativeDesktop = "native-desktop";
        private const string KeyNativeMobile = "native-mobile";

        public ClientApplicationTypesModel(Solution solution)
            : base(solution)
        {
            BrowserBasedApplication = GetBrowserBasedApplication(ClientApplication);
            NativeDesktopApplication = GetNativeDesktopApplication(ClientApplication);
            NativeMobileApplication = GetNativeMobileApplication(ClientApplication);

            ApplicationTypes = GetApplicationTypes();

            PaginationFooter.FullWidth = true;
        }

        [UIHint("DescriptionList")]
        public DescriptionListViewModel ApplicationTypes { get; init; }

        [UIHint("DescriptionList")]
        public DescriptionListViewModel BrowserBasedApplication { get; init; }

        public override int Index => 8;

        [UIHint("DescriptionList")]
        public DescriptionListViewModel NativeDesktopApplication { get; init; }

        [UIHint("DescriptionList")]
        public DescriptionListViewModel NativeMobileApplication { get; init; }

        public string HasApplicationType(string key) =>
            (ClientApplication?.ClientApplicationTypes?.Any(s => s.EqualsIgnoreCase(key))).ToYesNo();

        private static DescriptionListViewModel GetBrowserBasedApplication(ClientApplication clientApplication)
        {
            if (!clientApplication.ClientApplicationTypes.Any(t => t.EqualsIgnoreWhiteSpace(KeyBrowserBased)))
                return null;

            var items = new Dictionary<string, ListViewModel>();

            if (clientApplication.BrowsersSupported?.Any() == true)
            {
                items.Add(
                    "Supported browser types",
                    new ListViewModel { List = clientApplication.BrowsersSupported.ToArray(), });
            }

            if (clientApplication.MobileResponsive is not null)
            {
                items.Add(
                    "Mobile responsive",
                    new ListViewModel { Text = clientApplication.MobileResponsive.ToYesNo(), });
            }

            if (clientApplication.Plugins is not null)
            {
                items.Add(
                    "Plug-ins or extensions required",
                    new ListViewModel { Text = clientApplication.Plugins.Required.ToYesNo(), });

                if (!string.IsNullOrWhiteSpace(clientApplication.Plugins.AdditionalInformation))
                {
                    items.Add(
                        "Additional information about plug-ins or extensions",
                        new ListViewModel { Text = clientApplication.Plugins.AdditionalInformation, });
                }
            }

            if (!string.IsNullOrWhiteSpace(clientApplication.MinimumConnectionSpeed))
            {
                items.Add(
                    "Minimum connection speed",
                    new ListViewModel { Text = clientApplication.MinimumConnectionSpeed, });
            }

            if (!string.IsNullOrWhiteSpace(clientApplication.MinimumDesktopResolution))
            {
                items.Add(
                    "Screen resolution and aspect ratio",
                    new ListViewModel { Text = clientApplication.MinimumDesktopResolution, });
            }

            if (!string.IsNullOrWhiteSpace(clientApplication.HardwareRequirements))
            {
                items.Add(
                    "Hardware requirements",
                    new ListViewModel { Text = clientApplication.HardwareRequirements });
            }

            if (!string.IsNullOrWhiteSpace(clientApplication.AdditionalInformation))
            {
                items.Add(
                    "Additional information",
                    new ListViewModel { Text = clientApplication.AdditionalInformation });
            }

            return new DescriptionListViewModel
            {
                Heading = "Browser-based application",
                Items = items,
            };
        }

        private static DescriptionListViewModel GetNativeDesktopApplication(ClientApplication clientApplication)
        {
            if (!clientApplication.ClientApplicationTypes.Any(t => t.EqualsIgnoreWhiteSpace(KeyNativeDesktop)))
                return null;

            var items = new Dictionary<string, ListViewModel>();

            if (!string.IsNullOrWhiteSpace(clientApplication.NativeDesktopOperatingSystemsDescription))
            {
                items.Add(
                    "Description of supported operating systems",
                    new ListViewModel { Text = clientApplication.NativeDesktopOperatingSystemsDescription });
            }

            if (!string.IsNullOrWhiteSpace(clientApplication.NativeDesktopMinimumConnectionSpeed))
            {
                items.Add(
                    "Minimum connection speed",
                    new ListViewModel { Text = clientApplication.NativeDesktopMinimumConnectionSpeed });
            }

            if (!string.IsNullOrWhiteSpace(
                clientApplication.NativeDesktopMemoryAndStorage?.RecommendedResolution))
            {
                items.Add(
                    "Screen resolution and aspect ratio",
                    new ListViewModel
                    {
                        Text = clientApplication.NativeDesktopMemoryAndStorage.RecommendedResolution,
                    });
            }

            if (!string.IsNullOrWhiteSpace(
                clientApplication.NativeDesktopMemoryAndStorage?.MinimumMemoryRequirement))
            {
                items.Add(
                    "Memory size",
                    new ListViewModel
                    {
                        Text = clientApplication.NativeDesktopMemoryAndStorage.MinimumMemoryRequirement,
                    });
            }

            if (!string.IsNullOrWhiteSpace(
                clientApplication.NativeDesktopMemoryAndStorage?.StorageRequirementsDescription))
            {
                items.Add(
                    "Storage space",
                    new ListViewModel
                    {
                        Text = clientApplication.NativeDesktopMemoryAndStorage.StorageRequirementsDescription,
                    });
            }

            if (!string.IsNullOrWhiteSpace(clientApplication.NativeDesktopMemoryAndStorage?.MinimumCpu))
            {
                items.Add(
                    "Processing power",
                    new ListViewModel { Text = clientApplication.NativeDesktopMemoryAndStorage.MinimumCpu });
            }

            if (!string.IsNullOrWhiteSpace(clientApplication.NativeDesktopThirdParty?.ThirdPartyComponents))
            {
                items.Add(
                    "Third-party components",
                    new ListViewModel { Text = clientApplication.NativeDesktopThirdParty.ThirdPartyComponents });
            }

            if (!string.IsNullOrWhiteSpace(clientApplication.NativeDesktopThirdParty?.DeviceCapabilities))
            {
                items.Add(
                    "Device capabilities",
                    new ListViewModel { Text = clientApplication.NativeDesktopThirdParty.DeviceCapabilities });
            }

            if (!string.IsNullOrWhiteSpace(clientApplication.NativeDesktopHardwareRequirements))
            {
                items.Add(
                    "Hardware requirements",
                    new ListViewModel { Text = clientApplication.NativeDesktopHardwareRequirements });
            }

            if (!string.IsNullOrWhiteSpace(clientApplication.NativeDesktopAdditionalInformation))
            {
                items.Add(
                    "Additional information",
                    new ListViewModel { Text = clientApplication.NativeDesktopAdditionalInformation });
            }

            return new DescriptionListViewModel
            {
                Heading = "Desktop application",
                Items = items,
            };
        }

        private static DescriptionListViewModel GetNativeMobileApplication(ClientApplication clientApplication)
        {
            if (!clientApplication.ClientApplicationTypes.Any(t => t.EqualsIgnoreWhiteSpace(KeyNativeMobile)))
                return null;

            var items = new Dictionary<string, ListViewModel>();

            if (clientApplication.MobileOperatingSystems?.OperatingSystems?.Any() == true)
            {
                items.Add(
                    "Supported operating systems",
                    new ListViewModel { List = clientApplication.MobileOperatingSystems.OperatingSystems.ToArray(), });
            }

            if (!string.IsNullOrWhiteSpace(
                clientApplication.MobileOperatingSystems?.OperatingSystemsDescription))
            {
                items.Add(
                    "Description of supported operating systems",
                    new ListViewModel { Text = clientApplication.MobileOperatingSystems.OperatingSystemsDescription });
            }

            if (!string.IsNullOrWhiteSpace(
                clientApplication.MobileConnectionDetails?.MinimumConnectionSpeed))
            {
                items.Add(
                    "Minimum connection speed",
                    new ListViewModel { Text = clientApplication.MobileConnectionDetails.MinimumConnectionSpeed });
            }

            if (clientApplication.MobileConnectionDetails?.ConnectionType?.Any() == true)
            {
                items.Add(
                    "Connection types supported",
                    new ListViewModel { List = clientApplication.MobileConnectionDetails.ConnectionType.ToArray(), });
            }

            if (!string.IsNullOrWhiteSpace(clientApplication.MobileConnectionDetails?.Description))
            {
                items.Add(
                    "Connection requirements",
                    new ListViewModel { Text = clientApplication.MobileConnectionDetails.Description });
            }

            if (!string.IsNullOrWhiteSpace(
                clientApplication.MobileMemoryAndStorage?.MinimumMemoryRequirement))
            {
                items.Add(
                    "Memory size",
                    new ListViewModel { Text = clientApplication.MobileMemoryAndStorage.MinimumMemoryRequirement });
            }

            if (!string.IsNullOrWhiteSpace(clientApplication.MobileMemoryAndStorage?.Description))
            {
                items.Add(
                    "Storage space",
                    new ListViewModel { Text = clientApplication.MobileMemoryAndStorage.Description });
            }

            if (!string.IsNullOrWhiteSpace(clientApplication.MobileThirdParty?.ThirdPartyComponents))
            {
                items.Add(
                    "Third-party components",
                    new ListViewModel { Text = clientApplication.MobileThirdParty.ThirdPartyComponents });
            }

            if (!string.IsNullOrWhiteSpace(clientApplication.MobileThirdParty?.DeviceCapabilities))
            {
                items.Add(
                    "Device capabilities",
                    new ListViewModel { Text = clientApplication.MobileThirdParty.DeviceCapabilities });
            }

            if (!string.IsNullOrWhiteSpace(clientApplication.NativeMobileHardwareRequirements))
            {
                items.Add(
                    "Hardware requirements",
                    new ListViewModel { Text = clientApplication.NativeMobileHardwareRequirements });
            }

            if (!string.IsNullOrWhiteSpace(clientApplication.NativeMobileAdditionalInformation))
            {
                items.Add(
                    "Additional information",
                    new ListViewModel { Text = clientApplication.NativeMobileAdditionalInformation });
            }

            return new DescriptionListViewModel
            {
                Heading = "Mobile or tablet application",
                Items = items,
            };
        }

        private DescriptionListViewModel GetApplicationTypes()
        {
            var items = new Dictionary<string, ListViewModel>();

            if (ClientApplication.ClientApplicationTypes?.Any(t => t.EqualsIgnoreCase(KeyBrowserBased)) ?? false)
            {
                items.Add(
                    "Browser-based application",
                    new ListViewModel { Text = HasApplicationType(KeyBrowserBased) });
            }

            if (ClientApplication.ClientApplicationTypes?.Any(t => t.EqualsIgnoreCase(KeyNativeDesktop)) ?? false)
            {
                items.Add(
                    "Desktop application",
                    new ListViewModel { Text = HasApplicationType(KeyNativeDesktop) });
            }

            if (ClientApplication?.ClientApplicationTypes?.Any(t => t.EqualsIgnoreCase(KeyNativeMobile)) ?? false)
            {
                items.Add(
                    "Mobile or tablet application",
                    new ListViewModel { Text = HasApplicationType(KeyNativeMobile) });
            }

            return new DescriptionListViewModel
            {
                Heading = "Application type",
                Items = items,
            };
        }
    }
}
