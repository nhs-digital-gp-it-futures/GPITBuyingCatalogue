using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.MappingProfiles
{
    public class SolutionDetailsProfile : Profile
    {
        private const string KeyBrowserBased = "browser-based";
        private const string KeyNativeDesktop = "native-desktop";
        private const string KeyNativeMobile = "native-mobile";

        private static readonly Dictionary<int, Func<CatalogueItem, bool>> ShowFunctions = new()
        {
            { 0, catalogueItem => !string.IsNullOrWhiteSpace(catalogueItem.Solution?.FullDescription) },
            { 1, catalogueItem => !string.IsNullOrWhiteSpace(catalogueItem.Solution?.Features) },
            { 2, catalogueItem => catalogueItem.Solution?.SolutionCapabilities != null },
            { 7, catalogueItem => !string.IsNullOrWhiteSpace(catalogueItem.Solution?.ImplementationDetail) },
            { 8, catalogueItem => !string.IsNullOrWhiteSpace(catalogueItem.Solution?.ClientApplication) },
            { 9, catalogueItem => !string.IsNullOrWhiteSpace(catalogueItem.Solution?.Hosting) },
            { 10, catalogueItem => !string.IsNullOrWhiteSpace(catalogueItem.Solution?.ServiceLevelAgreement) },
        };

        public SolutionDetailsProfile()
        {
            CreateMap<CatalogueItem, ClientApplicationTypesModel>()
                .ForMember(
                    dest => dest.ApplicationTypes,
                    opt => opt.MapFrom(
                        (_, dest, _) => new DescriptionListViewModel
                        {
                            Heading = "Type of application",
                            Items = new Dictionary<string, ListViewModel>
                            {
                                {
                                    "Browser-based application",
                                    new ListViewModel { Text = dest.HasApplicationType(KeyBrowserBased) }
                                },
                                {
                                    "Desktop application",
                                    new ListViewModel { Text = dest.HasApplicationType(KeyNativeDesktop) }
                                },
                                {
                                    "Mobile application",
                                    new ListViewModel { Text = dest.HasApplicationType(KeyNativeMobile) }
                                },
                            },
                        }))
                .ForMember(
                    dest => dest.BrowserBasedApplication,
                    opt => opt.MapFrom(
                        (_, dest) => new DescriptionListViewModel
                        {
                            Heading = "Browser-based application",
                            Items = GetBrowserBasedItems(dest.ClientApplication),
                        }))
                .ForMember(
                    dest => dest.NativeDesktopApplication,
                    opt => opt.MapFrom(
                        (_, dest) => new DescriptionListViewModel
                        {
                            Heading = "Desktop application", Items = GetNativeDesktopItems(dest.ClientApplication),
                        }))
                .ForMember(
                    dest => dest.NativeMobileApplication,
                    opt => opt.MapFrom(
                        (_, dest) => new DescriptionListViewModel
                        {
                            Heading = "Mobile application", Items = GetNativeMobileItems(dest.ClientApplication),
                        }))
                .ForMember(dest => dest.Section, opt => opt.MapFrom(src => "Client application type"))
                .IncludeBase<CatalogueItem, SolutionDisplayBaseModel>();

            CreateMap<CatalogueItem, SolutionDisplayBaseModel>()
                .BeforeMap(
                    (src, dest) => dest.ClientApplication =
                        src.Solution != null && !string.IsNullOrEmpty(src.Solution.ClientApplication)
                            ? JsonConvert.DeserializeObject<ClientApplication>(src.Solution.ClientApplication)
                            : new ClientApplication())
                .ForMember(dest => dest.ClientApplication, opt => opt.Ignore())
                .ForMember(
                    dest => dest.LastReviewed,
                    opt => opt.MapFrom<IMemberValueResolver<object, object, string, string>, string>(
                        x => "SolutionsLastReviewedDate"))
                .ForMember(dest => dest.PaginationFooter, opt => opt.Ignore())
                .ForMember(dest => dest.Section, opt => opt.Ignore())
                .ForMember(dest => dest.SolutionId, opt => opt.MapFrom(src => src.CatalogueItemId))
                .ForMember(dest => dest.SolutionName, opt => opt.MapFrom(src => src.Name))
                .IgnoreAllPropertiesWithAnInaccessibleSetter()
                .AfterMap(
                    (src, dest) =>
                    {
                        foreach (var (index, predicate) in ShowFunctions)
                        {
                            if (predicate(src))
                            {
                                dest.SetShowTrue(index);
                            }
                        }

                        var (previous, next) = dest.PreviousAndNextModels();
                        dest.PaginationFooter = new PaginationFooterModel { Next = next, Previous = previous, };
                    });

            CreateMap<CatalogueItem, ImplementationTimescalesModel>()
                .ForMember(
                    dest => dest.Description,
                    opt => opt.MapFrom(src => src.Solution == null ? null : src.Solution.ImplementationDetail))
                .ForMember(dest => dest.Section, opt => opt.MapFrom(src => "Implementation timescales"))
                .IncludeBase<CatalogueItem, SolutionDisplayBaseModel>();

            CreateMap<CatalogueItem, SolutionDescriptionModel>()
                .ForMember(
                    dest => dest.Description,
                    opt => opt.MapFrom(src => src.Solution == null ? null : src.Solution.FullDescription))
                .ForMember(dest => dest.Framework, opt => opt.MapFrom(src => src.Framework()))
                .ForMember(dest => dest.IsFoundation, opt => opt.MapFrom(src => src.IsFoundation().ToYesNo()))
                .ForMember(dest => dest.Section, opt => opt.MapFrom(src => "Description"))
                .ForMember(
                    dest => dest.Summary,
                    opt => opt.MapFrom(src => src.Solution == null ? null : src.Solution.Summary))
                .ForMember(
                    dest => dest.SupplierName,
                    opt => opt.MapFrom(src => src.Supplier == null ? null : src.Supplier.Name))
                .IncludeBase<CatalogueItem, SolutionDisplayBaseModel>();

            CreateMap<CatalogueItem, SolutionFeaturesModel>()
                .ForMember(dest => dest.Features, opt => opt.MapFrom(src => src.Features()))
                .ForMember(dest => dest.Section, opt => opt.MapFrom(src => "Features"))
                .IncludeBase<CatalogueItem, SolutionDisplayBaseModel>();
        }

        private static IDictionary<string, ListViewModel> GetBrowserBasedItems(ClientApplication clientApplication)
        {
            var result = new Dictionary<string, ListViewModel>();

            if (clientApplication.BrowsersSupported?.Any() == true)
            {
                result.Add(
                    "Supported browser types",
                    new ListViewModel { List = clientApplication.BrowsersSupported.ToArray(), });
            }

            if (clientApplication.MobileResponsive is not null)
            {
                result.Add(
                    "Mobile responsive",
                    new ListViewModel { Text = clientApplication.MobileResponsive.ToYesNo(), });
            }

            if (clientApplication.Plugins is not null)
            {
                result.Add(
                    "Plug-ins or extensions required",
                    new ListViewModel { Text = clientApplication.Plugins.Required.ToYesNo(), });

                if (!string.IsNullOrWhiteSpace(clientApplication.Plugins.AdditionalInformation))
                {
                    result.Add(
                        "Additional information about plug-ins or extensions",
                        new ListViewModel { Text = clientApplication.Plugins.AdditionalInformation, });
                }
            }

            if (!string.IsNullOrWhiteSpace(clientApplication.MinimumConnectionSpeed))
            {
                result.Add(
                    "Minimum connection speed",
                    new ListViewModel { Text = clientApplication.MinimumConnectionSpeed, });
            }

            if (!string.IsNullOrWhiteSpace(clientApplication.MinimumDesktopResolution))
            {
                result.Add(
                    "Screen resolution and aspect ratio",
                    new ListViewModel { Text = clientApplication.MinimumDesktopResolution, });
            }

            if (!string.IsNullOrWhiteSpace(clientApplication.HardwareRequirements))
            {
                result.Add(
                    "Hardware requirements",
                    new ListViewModel { Text = clientApplication.HardwareRequirements });
            }

            if (!string.IsNullOrWhiteSpace(clientApplication.AdditionalInformation))
            {
                result.Add(
                    "Additional information",
                    new ListViewModel { Text = clientApplication.AdditionalInformation });
            }

            return result;
        }

        private static IDictionary<string, ListViewModel> GetNativeDesktopItems(ClientApplication clientApplication)
        {
            var result = new Dictionary<string, ListViewModel>();
            if (!string.IsNullOrWhiteSpace(clientApplication.NativeDesktopOperatingSystemsDescription))
            {
                result.Add(
                    "Description of supported operating systems",
                    new ListViewModel { Text = clientApplication.NativeDesktopOperatingSystemsDescription });
            }

            if (!string.IsNullOrWhiteSpace(clientApplication.NativeDesktopMinimumConnectionSpeed))
            {
                result.Add(
                    "Minimum connection speed",
                    new ListViewModel { Text = clientApplication.NativeDesktopMinimumConnectionSpeed });
            }

            if (!string.IsNullOrWhiteSpace(
                clientApplication.NativeDesktopMemoryAndStorage?.RecommendedResolution))
            {
                result.Add(
                    "Screen resolution and aspect ratio",
                    new ListViewModel
                    {
                        Text = clientApplication.NativeDesktopMemoryAndStorage.RecommendedResolution,
                    });
            }

            if (!string.IsNullOrWhiteSpace(
                clientApplication.NativeDesktopMemoryAndStorage?.MinimumMemoryRequirement))
            {
                result.Add(
                    "Memory size",
                    new ListViewModel
                    {
                        Text = clientApplication.NativeDesktopMemoryAndStorage.MinimumMemoryRequirement,
                    });
            }

            if (!string.IsNullOrWhiteSpace(
                clientApplication.NativeDesktopMemoryAndStorage?.StorageRequirementsDescription))
            {
                result.Add(
                    "Storage space",
                    new ListViewModel
                    {
                        Text = clientApplication.NativeDesktopMemoryAndStorage.StorageRequirementsDescription,
                    });
            }

            if (!string.IsNullOrWhiteSpace(clientApplication.NativeDesktopMemoryAndStorage?.MinimumCpu))
            {
                result.Add(
                    "Processing power",
                    new ListViewModel { Text = clientApplication.NativeDesktopMemoryAndStorage.MinimumCpu });
            }

            if (!string.IsNullOrWhiteSpace(clientApplication.NativeDesktopThirdParty?.ThirdPartyComponents))
            {
                result.Add(
                    "Third-party components",
                    new ListViewModel { Text = clientApplication.NativeDesktopThirdParty.ThirdPartyComponents });
            }

            if (!string.IsNullOrWhiteSpace(clientApplication.NativeDesktopThirdParty?.DeviceCapabilities))
            {
                result.Add(
                    "Device capabilities",
                    new ListViewModel { Text = clientApplication.NativeDesktopThirdParty.DeviceCapabilities });
            }

            if (!string.IsNullOrWhiteSpace(clientApplication.NativeDesktopHardwareRequirements))
            {
                result.Add(
                    "Hardware requirements",
                    new ListViewModel { Text = clientApplication.NativeDesktopHardwareRequirements });
            }

            if (!string.IsNullOrWhiteSpace(clientApplication.NativeDesktopAdditionalInformation))
            {
                result.Add(
                    "Additional information",
                    new ListViewModel { Text = clientApplication.NativeDesktopAdditionalInformation });
            }

            return result;
        }

        private static IDictionary<string, ListViewModel> GetNativeMobileItems(ClientApplication clientApplication)
        {
            var result = new Dictionary<string, ListViewModel>();

            if (clientApplication.MobileOperatingSystems?.OperatingSystems?.Any() == true)
            {
                result.Add(
                    "Supported operating systems",
                    new ListViewModel { List = clientApplication.MobileOperatingSystems.OperatingSystems.ToArray(), });
            }

            if (!string.IsNullOrWhiteSpace(
                clientApplication.MobileOperatingSystems?.OperatingSystemsDescription))
            {
                result.Add(
                    "Description of supported operating systems",
                    new ListViewModel { Text = clientApplication.MobileOperatingSystems.OperatingSystemsDescription });
            }

            if (!string.IsNullOrWhiteSpace(
                clientApplication.MobileConnectionDetails?.MinimumConnectionSpeed))
            {
                result.Add(
                    "Minimum connection speed",
                    new ListViewModel { Text = clientApplication.MobileConnectionDetails.MinimumConnectionSpeed });
            }

            if (clientApplication.MobileConnectionDetails?.ConnectionType?.Any() == true)
            {
                result.Add(
                    "Connection types supported",
                    new ListViewModel { List = clientApplication.MobileConnectionDetails.ConnectionType.ToArray(), });
            }

            if (!string.IsNullOrWhiteSpace(clientApplication.MobileConnectionDetails?.Description))
            {
                result.Add(
                    "Connection requirements",
                    new ListViewModel { Text = clientApplication.MobileConnectionDetails.Description });
            }

            if (!string.IsNullOrWhiteSpace(
                clientApplication.MobileMemoryAndStorage?.MinimumMemoryRequirement))
            {
                result.Add(
                    "Memory size",
                    new ListViewModel { Text = clientApplication.MobileMemoryAndStorage.MinimumMemoryRequirement });
            }

            if (!string.IsNullOrWhiteSpace(clientApplication.MobileMemoryAndStorage?.Description))
            {
                result.Add(
                    "Storage space",
                    new ListViewModel { Text = clientApplication.MobileMemoryAndStorage.Description });
            }

            if (!string.IsNullOrWhiteSpace(clientApplication.MobileThirdParty?.ThirdPartyComponents))
            {
                result.Add(
                    "Third-party components",
                    new ListViewModel { Text = clientApplication.MobileThirdParty.ThirdPartyComponents });
            }

            if (!string.IsNullOrWhiteSpace(clientApplication.MobileThirdParty?.DeviceCapabilities))
            {
                result.Add(
                    "Device capabilities",
                    new ListViewModel { Text = clientApplication.MobileThirdParty.DeviceCapabilities });
            }

            if (!string.IsNullOrWhiteSpace(clientApplication.NativeMobileHardwareRequirements))
            {
                result.Add(
                    "Hardware requirements",
                    new ListViewModel { Text = clientApplication.NativeMobileHardwareRequirements });
            }

            if (!string.IsNullOrWhiteSpace(clientApplication.NativeMobileAdditionalInformation))
            {
                result.Add(
                    "Additional information",
                    new ListViewModel { Text = clientApplication.NativeMobileAdditionalInformation });
            }

            return result;
        }
    }
}
