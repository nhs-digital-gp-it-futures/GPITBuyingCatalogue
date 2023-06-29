using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public sealed class ApplicationTypesModel : SolutionDisplayBaseModel
    {
        public ApplicationTypesModel(
            CatalogueItem catalogueItem,
            CatalogueItemContentStatus contentStatus)
            : base(catalogueItem, contentStatus)
        {
            ApplicationTypeDetail = catalogueItem.Solution.EnsureAndGetApplicationType();

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

        public ApplicationTypeDetail ApplicationTypeDetail { get; init; }

        public bool HasApplicationType(ApplicationType applicationType) =>
            ApplicationTypeDetail?.ApplicationTypes?.Any(
                s => s.EqualsIgnoreCase(applicationType.EnumMemberName())) ?? false;

        private DescriptionListViewModel GetBrowserBasedApplication()
        {
            if (!HasApplicationType(ApplicationType.BrowserBased))
                return null;

            var items = new Dictionary<string, ListViewModel>();

            if (ApplicationTypeDetail.BrowsersSupported?.Any() == true)
            {
                items.Add(
                    "Supported browser types",
                    new ListViewModel
                    {
                        List = ApplicationTypeDetail
                        .BrowsersSupported
                        .Select(bs =>
                        !string.IsNullOrWhiteSpace(bs.MinimumBrowserVersion)
                        ? $"{bs.BrowserName} (Version {bs.MinimumBrowserVersion})"
                        : bs.BrowserName).ToArray(),
                    });
            }

            if (ApplicationTypeDetail.MobileResponsive is not null)
            {
                items.Add(
                    "Mobile responsive",
                    new ListViewModel { Text = ApplicationTypeDetail.MobileResponsive.ToYesNo(), });
            }

            if (ApplicationTypeDetail.Plugins is not null)
            {
                items.Add(
                    "Plug-ins or extensions required",
                    new ListViewModel { Text = ApplicationTypeDetail.Plugins.Required.ToYesNo(), });

                if (!string.IsNullOrWhiteSpace(ApplicationTypeDetail.Plugins.AdditionalInformation))
                {
                    items.Add(
                        "Additional information about plug-ins or extensions",
                        new ListViewModel { Text = ApplicationTypeDetail.Plugins.AdditionalInformation, });
                }
            }

            if (!string.IsNullOrWhiteSpace(ApplicationTypeDetail.MinimumConnectionSpeed))
            {
                items.Add(
                    "Minimum connection speed",
                    new ListViewModel { Text = ApplicationTypeDetail.MinimumConnectionSpeed, });
            }

            if (!string.IsNullOrWhiteSpace(ApplicationTypeDetail.MinimumDesktopResolution))
            {
                items.Add(
                    "Screen resolution and aspect ratio",
                    new ListViewModel { Text = ApplicationTypeDetail.MinimumDesktopResolution, });
            }

            if (!string.IsNullOrWhiteSpace(ApplicationTypeDetail.HardwareRequirements))
            {
                items.Add(
                    "Hardware requirements",
                    new ListViewModel { Text = ApplicationTypeDetail.HardwareRequirements });
            }

            if (!string.IsNullOrWhiteSpace(ApplicationTypeDetail.AdditionalInformation))
            {
                items.Add(
                    "Additional information",
                    new ListViewModel { Text = ApplicationTypeDetail.AdditionalInformation });
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

            if (!string.IsNullOrWhiteSpace(ApplicationTypeDetail.NativeDesktopOperatingSystemsDescription))
            {
                items.Add(
                    "Description of supported operating systems",
                    new ListViewModel { Text = ApplicationTypeDetail.NativeDesktopOperatingSystemsDescription });
            }

            if (!string.IsNullOrWhiteSpace(ApplicationTypeDetail.NativeDesktopMinimumConnectionSpeed))
            {
                items.Add(
                    "Minimum connection speed",
                    new ListViewModel { Text = ApplicationTypeDetail.NativeDesktopMinimumConnectionSpeed });
            }

            if (!string.IsNullOrWhiteSpace(
                ApplicationTypeDetail.NativeDesktopMemoryAndStorage?.RecommendedResolution))
            {
                items.Add(
                    "Screen resolution and aspect ratio",
                    new ListViewModel
                    {
                        Text = ApplicationTypeDetail.NativeDesktopMemoryAndStorage.RecommendedResolution,
                    });
            }

            if (!string.IsNullOrWhiteSpace(
                ApplicationTypeDetail.NativeDesktopMemoryAndStorage?.MinimumMemoryRequirement))
            {
                items.Add(
                    "Memory size",
                    new ListViewModel
                    {
                        Text = ApplicationTypeDetail.NativeDesktopMemoryAndStorage.MinimumMemoryRequirement,
                    });
            }

            if (!string.IsNullOrWhiteSpace(
                ApplicationTypeDetail.NativeDesktopMemoryAndStorage?.StorageRequirementsDescription))
            {
                items.Add(
                    "Storage space",
                    new ListViewModel
                    {
                        Text = ApplicationTypeDetail.NativeDesktopMemoryAndStorage.StorageRequirementsDescription,
                    });
            }

            if (!string.IsNullOrWhiteSpace(ApplicationTypeDetail.NativeDesktopMemoryAndStorage?.MinimumCpu))
            {
                items.Add(
                    "Processing power",
                    new ListViewModel { Text = ApplicationTypeDetail.NativeDesktopMemoryAndStorage.MinimumCpu });
            }

            if (!string.IsNullOrWhiteSpace(ApplicationTypeDetail.NativeDesktopThirdParty?.ThirdPartyComponents))
            {
                items.Add(
                    "Third-party components",
                    new ListViewModel { Text = ApplicationTypeDetail.NativeDesktopThirdParty.ThirdPartyComponents });
            }

            if (!string.IsNullOrWhiteSpace(ApplicationTypeDetail.NativeDesktopThirdParty?.DeviceCapabilities))
            {
                items.Add(
                    "Device capabilities",
                    new ListViewModel { Text = ApplicationTypeDetail.NativeDesktopThirdParty.DeviceCapabilities });
            }

            if (!string.IsNullOrWhiteSpace(ApplicationTypeDetail.NativeDesktopHardwareRequirements))
            {
                items.Add(
                    "Hardware requirements",
                    new ListViewModel { Text = ApplicationTypeDetail.NativeDesktopHardwareRequirements });
            }

            if (!string.IsNullOrWhiteSpace(ApplicationTypeDetail.NativeDesktopAdditionalInformation))
            {
                items.Add(
                    "Additional information",
                    new ListViewModel { Text = ApplicationTypeDetail.NativeDesktopAdditionalInformation });
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

            if (ApplicationTypeDetail.MobileOperatingSystems?.OperatingSystems?.Any() == true)
            {
                items.Add(
                    "Supported operating systems",
                    new ListViewModel { List = ApplicationTypeDetail.MobileOperatingSystems.OperatingSystems.ToArray(), });
            }

            if (!string.IsNullOrWhiteSpace(
                ApplicationTypeDetail.MobileOperatingSystems?.OperatingSystemsDescription))
            {
                items.Add(
                    "Description of supported operating systems",
                    new ListViewModel { Text = ApplicationTypeDetail.MobileOperatingSystems.OperatingSystemsDescription });
            }

            if (!string.IsNullOrWhiteSpace(
                ApplicationTypeDetail.MobileConnectionDetails?.MinimumConnectionSpeed))
            {
                items.Add(
                    "Minimum connection speed",
                    new ListViewModel { Text = ApplicationTypeDetail.MobileConnectionDetails.MinimumConnectionSpeed });
            }

            if (ApplicationTypeDetail.MobileConnectionDetails?.ConnectionType?.Any() == true)
            {
                items.Add(
                    "Connection types supported",
                    new ListViewModel { List = ApplicationTypeDetail.MobileConnectionDetails.ConnectionType.ToArray(), });
            }

            if (!string.IsNullOrWhiteSpace(ApplicationTypeDetail.MobileConnectionDetails?.Description))
            {
                items.Add(
                    "Connection requirements",
                    new ListViewModel { Text = ApplicationTypeDetail.MobileConnectionDetails.Description });
            }

            if (!string.IsNullOrWhiteSpace(
                ApplicationTypeDetail.MobileMemoryAndStorage?.MinimumMemoryRequirement))
            {
                items.Add(
                    "Memory size",
                    new ListViewModel { Text = ApplicationTypeDetail.MobileMemoryAndStorage.MinimumMemoryRequirement });
            }

            if (!string.IsNullOrWhiteSpace(ApplicationTypeDetail.MobileMemoryAndStorage?.Description))
            {
                items.Add(
                    "Storage space",
                    new ListViewModel { Text = ApplicationTypeDetail.MobileMemoryAndStorage.Description });
            }

            if (!string.IsNullOrWhiteSpace(ApplicationTypeDetail.MobileThirdParty?.ThirdPartyComponents))
            {
                items.Add(
                    "Third-party components",
                    new ListViewModel { Text = ApplicationTypeDetail.MobileThirdParty.ThirdPartyComponents });
            }

            if (!string.IsNullOrWhiteSpace(ApplicationTypeDetail.MobileThirdParty?.DeviceCapabilities))
            {
                items.Add(
                    "Device capabilities",
                    new ListViewModel { Text = ApplicationTypeDetail.MobileThirdParty.DeviceCapabilities });
            }

            if (!string.IsNullOrWhiteSpace(ApplicationTypeDetail.NativeMobileHardwareRequirements))
            {
                items.Add(
                    "Hardware requirements",
                    new ListViewModel { Text = ApplicationTypeDetail.NativeMobileHardwareRequirements });
            }

            if (!string.IsNullOrWhiteSpace(ApplicationTypeDetail.NativeMobileAdditionalInformation))
            {
                items.Add(
                    "Additional information",
                    new ListViewModel { Text = ApplicationTypeDetail.NativeMobileAdditionalInformation });
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
