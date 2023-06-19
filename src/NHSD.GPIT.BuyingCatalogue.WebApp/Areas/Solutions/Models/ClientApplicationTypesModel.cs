using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public sealed class ClientApplicationTypesModel : SolutionDisplayBaseModel
    {
        public ClientApplicationTypesModel(
            CatalogueItem catalogueItem,
            CatalogueItemContentStatus contentStatus)
            : base(catalogueItem, contentStatus)
        {
            ClientApplication = catalogueItem.Solution.EnsureAndGetApplicationType();

            BrowserBasedApplication = GetBrowserBasedApplication();
            NativeDesktopApplication = GetNativeDesktopApplication();
            NativeMobileApplication = GetNativeMobileApplication();

            ApplicationTypes = GetApplicationTypes();

            PaginationFooter.FullWidth = true;
        }

        public override int Index => 9;

        [UIHint("DescriptionList")]
        public DescriptionListViewModel ApplicationTypes { get; init; }

        [UIHint("DescriptionList")]
        public DescriptionListViewModel BrowserBasedApplication { get; init; }

        [UIHint("DescriptionList")]
        public DescriptionListViewModel NativeDesktopApplication { get; init; }

        [UIHint("DescriptionList")]
        public DescriptionListViewModel NativeMobileApplication { get; init; }

        public ApplicationTypeDetail ClientApplication { get; init; }

        public bool HasApplicationType(ApplicationType clientApplicationType) =>
            ClientApplication?.ClientApplicationTypes?.Any(
                s => s.EqualsIgnoreCase(clientApplicationType.EnumMemberName())) ?? false;

        private DescriptionListViewModel GetBrowserBasedApplication()
        {
            if (!HasApplicationType(ApplicationType.BrowserBased))
                return null;

            var items = new Dictionary<string, ListViewModel>();

            if (ClientApplication.BrowsersSupported?.Any() == true)
            {
                items.Add(
                    "Supported browser types",
                    new ListViewModel
                    {
                        List = ClientApplication
                        .BrowsersSupported
                        .Select(bs =>
                        !string.IsNullOrWhiteSpace(bs.MinimumBrowserVersion)
                        ? $"{bs.BrowserName} (Version {bs.MinimumBrowserVersion})"
                        : bs.BrowserName).ToArray(),
                    });
            }

            if (ClientApplication.MobileResponsive is not null)
            {
                items.Add(
                    "Mobile responsive",
                    new ListViewModel { Text = ClientApplication.MobileResponsive.ToYesNo(), });
            }

            if (ClientApplication.Plugins is not null)
            {
                items.Add(
                    "Plug-ins or extensions required",
                    new ListViewModel { Text = ClientApplication.Plugins.Required.ToYesNo(), });

                if (!string.IsNullOrWhiteSpace(ClientApplication.Plugins.AdditionalInformation))
                {
                    items.Add(
                        "Additional information about plug-ins or extensions",
                        new ListViewModel { Text = ClientApplication.Plugins.AdditionalInformation, });
                }
            }

            if (!string.IsNullOrWhiteSpace(ClientApplication.MinimumConnectionSpeed))
            {
                items.Add(
                    "Minimum connection speed",
                    new ListViewModel { Text = ClientApplication.MinimumConnectionSpeed, });
            }

            if (!string.IsNullOrWhiteSpace(ClientApplication.MinimumDesktopResolution))
            {
                items.Add(
                    "Screen resolution and aspect ratio",
                    new ListViewModel { Text = ClientApplication.MinimumDesktopResolution, });
            }

            if (!string.IsNullOrWhiteSpace(ClientApplication.HardwareRequirements))
            {
                items.Add(
                    "Hardware requirements",
                    new ListViewModel { Text = ClientApplication.HardwareRequirements });
            }

            if (!string.IsNullOrWhiteSpace(ClientApplication.AdditionalInformation))
            {
                items.Add(
                    "Additional information",
                    new ListViewModel { Text = ClientApplication.AdditionalInformation });
            }

            return new DescriptionListViewModel
            {
                Heading = "Browser-based application",
                Items = items,
            };
        }

        private DescriptionListViewModel GetNativeDesktopApplication()
        {
            if (!HasApplicationType(ApplicationType.Desktop))
                return null;

            var items = new Dictionary<string, ListViewModel>();

            if (!string.IsNullOrWhiteSpace(ClientApplication.NativeDesktopOperatingSystemsDescription))
            {
                items.Add(
                    "Description of supported operating systems",
                    new ListViewModel { Text = ClientApplication.NativeDesktopOperatingSystemsDescription });
            }

            if (!string.IsNullOrWhiteSpace(ClientApplication.NativeDesktopMinimumConnectionSpeed))
            {
                items.Add(
                    "Minimum connection speed",
                    new ListViewModel { Text = ClientApplication.NativeDesktopMinimumConnectionSpeed });
            }

            if (!string.IsNullOrWhiteSpace(
                ClientApplication.NativeDesktopMemoryAndStorage?.RecommendedResolution))
            {
                items.Add(
                    "Screen resolution and aspect ratio",
                    new ListViewModel
                    {
                        Text = ClientApplication.NativeDesktopMemoryAndStorage.RecommendedResolution,
                    });
            }

            if (!string.IsNullOrWhiteSpace(
                ClientApplication.NativeDesktopMemoryAndStorage?.MinimumMemoryRequirement))
            {
                items.Add(
                    "Memory size",
                    new ListViewModel
                    {
                        Text = ClientApplication.NativeDesktopMemoryAndStorage.MinimumMemoryRequirement,
                    });
            }

            if (!string.IsNullOrWhiteSpace(
                ClientApplication.NativeDesktopMemoryAndStorage?.StorageRequirementsDescription))
            {
                items.Add(
                    "Storage space",
                    new ListViewModel
                    {
                        Text = ClientApplication.NativeDesktopMemoryAndStorage.StorageRequirementsDescription,
                    });
            }

            if (!string.IsNullOrWhiteSpace(ClientApplication.NativeDesktopMemoryAndStorage?.MinimumCpu))
            {
                items.Add(
                    "Processing power",
                    new ListViewModel { Text = ClientApplication.NativeDesktopMemoryAndStorage.MinimumCpu });
            }

            if (!string.IsNullOrWhiteSpace(ClientApplication.NativeDesktopThirdParty?.ThirdPartyComponents))
            {
                items.Add(
                    "Third-party components",
                    new ListViewModel { Text = ClientApplication.NativeDesktopThirdParty.ThirdPartyComponents });
            }

            if (!string.IsNullOrWhiteSpace(ClientApplication.NativeDesktopThirdParty?.DeviceCapabilities))
            {
                items.Add(
                    "Device capabilities",
                    new ListViewModel { Text = ClientApplication.NativeDesktopThirdParty.DeviceCapabilities });
            }

            if (!string.IsNullOrWhiteSpace(ClientApplication.NativeDesktopHardwareRequirements))
            {
                items.Add(
                    "Hardware requirements",
                    new ListViewModel { Text = ClientApplication.NativeDesktopHardwareRequirements });
            }

            if (!string.IsNullOrWhiteSpace(ClientApplication.NativeDesktopAdditionalInformation))
            {
                items.Add(
                    "Additional information",
                    new ListViewModel { Text = ClientApplication.NativeDesktopAdditionalInformation });
            }

            return new DescriptionListViewModel
            {
                Heading = "Desktop application",
                Items = items,
            };
        }

        private DescriptionListViewModel GetNativeMobileApplication()
        {
            if (!HasApplicationType(ApplicationType.MobileTablet))
                return null;

            var items = new Dictionary<string, ListViewModel>();

            if (ClientApplication.MobileOperatingSystems?.OperatingSystems?.Any() == true)
            {
                items.Add(
                    "Supported operating systems",
                    new ListViewModel { List = ClientApplication.MobileOperatingSystems.OperatingSystems.ToArray(), });
            }

            if (!string.IsNullOrWhiteSpace(
                ClientApplication.MobileOperatingSystems?.OperatingSystemsDescription))
            {
                items.Add(
                    "Description of supported operating systems",
                    new ListViewModel { Text = ClientApplication.MobileOperatingSystems.OperatingSystemsDescription });
            }

            if (!string.IsNullOrWhiteSpace(
                ClientApplication.MobileConnectionDetails?.MinimumConnectionSpeed))
            {
                items.Add(
                    "Minimum connection speed",
                    new ListViewModel { Text = ClientApplication.MobileConnectionDetails.MinimumConnectionSpeed });
            }

            if (ClientApplication.MobileConnectionDetails?.ConnectionType?.Any() == true)
            {
                items.Add(
                    "Connection types supported",
                    new ListViewModel { List = ClientApplication.MobileConnectionDetails.ConnectionType.ToArray(), });
            }

            if (!string.IsNullOrWhiteSpace(ClientApplication.MobileConnectionDetails?.Description))
            {
                items.Add(
                    "Connection requirements",
                    new ListViewModel { Text = ClientApplication.MobileConnectionDetails.Description });
            }

            if (!string.IsNullOrWhiteSpace(
                ClientApplication.MobileMemoryAndStorage?.MinimumMemoryRequirement))
            {
                items.Add(
                    "Memory size",
                    new ListViewModel { Text = ClientApplication.MobileMemoryAndStorage.MinimumMemoryRequirement });
            }

            if (!string.IsNullOrWhiteSpace(ClientApplication.MobileMemoryAndStorage?.Description))
            {
                items.Add(
                    "Storage space",
                    new ListViewModel { Text = ClientApplication.MobileMemoryAndStorage.Description });
            }

            if (!string.IsNullOrWhiteSpace(ClientApplication.MobileThirdParty?.ThirdPartyComponents))
            {
                items.Add(
                    "Third-party components",
                    new ListViewModel { Text = ClientApplication.MobileThirdParty.ThirdPartyComponents });
            }

            if (!string.IsNullOrWhiteSpace(ClientApplication.MobileThirdParty?.DeviceCapabilities))
            {
                items.Add(
                    "Device capabilities",
                    new ListViewModel { Text = ClientApplication.MobileThirdParty.DeviceCapabilities });
            }

            if (!string.IsNullOrWhiteSpace(ClientApplication.NativeMobileHardwareRequirements))
            {
                items.Add(
                    "Hardware requirements",
                    new ListViewModel { Text = ClientApplication.NativeMobileHardwareRequirements });
            }

            if (!string.IsNullOrWhiteSpace(ClientApplication.NativeMobileAdditionalInformation))
            {
                items.Add(
                    "Additional information",
                    new ListViewModel { Text = ClientApplication.NativeMobileAdditionalInformation });
            }

            return new DescriptionListViewModel
            {
                Heading = "Mobile or tablet application",
                Items = items,
            };
        }

        private DescriptionListViewModel GetApplicationTypes()
        {
            const string yesKey = "Yes";
            var items = new Dictionary<string, ListViewModel>();

            if (HasApplicationType(ApplicationType.BrowserBased))
            {
                items.Add(
                    "Browser-based application",
                    new ListViewModel { Text = yesKey });
            }

            if (HasApplicationType(ApplicationType.Desktop))
            {
                items.Add(
                    "Desktop application",
                    new ListViewModel { Text = yesKey });
            }

            if (HasApplicationType(ApplicationType.MobileTablet))
            {
                items.Add(
                    "Mobile or tablet application",
                    new ListViewModel { Text = yesKey });
            }

            return new DescriptionListViewModel
            {
                Heading = "Application type",
                Items = items,
            };
        }
    }
}
