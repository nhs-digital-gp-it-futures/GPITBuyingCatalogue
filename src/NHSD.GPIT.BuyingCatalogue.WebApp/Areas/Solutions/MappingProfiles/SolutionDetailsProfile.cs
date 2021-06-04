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
                .ForMember(dest => dest.BrowserBasedApplication, opt => opt.Ignore())
                .ForMember(dest => dest.NativeDesktopApplication, opt => opt.Ignore())
                .ForMember(dest => dest.NativeMobileApplication, opt => opt.Ignore())
                .ForMember(dest => dest.Section, opt => opt.MapFrom(src => "Client application type"))
                .IncludeBase<CatalogueItem, SolutionDisplayBaseModel>()
                .AfterMap(
                    (_, dest) =>
                    {
                        dest.BrowserBasedApplication = new DescriptionListViewModel
                        {
                            Heading = "Browser-based application",
                        };
                        if (dest.ClientApplication == null)
                            return;

                        if (dest.ClientApplication.BrowsersSupported?.Any() == true)
                        {
                            dest.BrowserBasedApplication.Items.Add(
                                "Supported browser types",
                                new ListViewModel { List = dest.ClientApplication.BrowsersSupported.ToArray(), });
                        }

                        if (!(dest.ClientApplication.MobileResponsive is null))
                        {
                            dest.BrowserBasedApplication.Items.Add(
                                "Mobile responsive",
                                new ListViewModel { Text = dest.ClientApplication.MobileResponsive.ToYesNo(), });
                        }

                        if (!(dest.ClientApplication.Plugins == null))
                        {
                            dest.BrowserBasedApplication.Items.Add(
                                "Plug-ins or extensions required",
                                new ListViewModel { Text = dest.ClientApplication.Plugins.Required.ToYesNo(), });

                            if (!string.IsNullOrWhiteSpace(dest.ClientApplication.Plugins.AdditionalInformation))
                            {
                                dest.BrowserBasedApplication.Items.Add(
                                    "Additional information about plug-ins or extensions",
                                    new ListViewModel { Text = dest.ClientApplication.Plugins.AdditionalInformation, });
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(dest.ClientApplication.MinimumConnectionSpeed))
                        {
                            dest.BrowserBasedApplication.Items.Add(
                                "Minimum connection speed",
                                new ListViewModel { Text = dest.ClientApplication.MinimumConnectionSpeed, });
                        }

                        if (!string.IsNullOrWhiteSpace(dest.ClientApplication.MinimumDesktopResolution))
                        {
                            dest.BrowserBasedApplication.Items.Add(
                                "Screen resolution and aspect ratio",
                                new ListViewModel { Text = dest.ClientApplication.MinimumDesktopResolution, });
                        }

                        if (!string.IsNullOrWhiteSpace(dest.ClientApplication.HardwareRequirements))
                        {
                            dest.BrowserBasedApplication.Items.Add(
                                "Hardware requirements",
                                new ListViewModel { Text = dest.ClientApplication.HardwareRequirements });
                        }

                        if (!string.IsNullOrWhiteSpace(dest.ClientApplication.AdditionalInformation))
                        {
                            dest.BrowserBasedApplication.Items.Add(
                                "Additional information",
                                new ListViewModel { Text = dest.ClientApplication.AdditionalInformation });
                        }

                        dest.NativeDesktopApplication = new DescriptionListViewModel
                        {
                            Heading = "Desktop application",
                        };

                        if (!string.IsNullOrWhiteSpace(dest.ClientApplication.NativeDesktopOperatingSystemsDescription))
                        {
                            dest.NativeDesktopApplication.Items.Add(
                                "Description of supported operating systems",
                                new ListViewModel { Text = dest.ClientApplication.NativeDesktopOperatingSystemsDescription });
                        }

                        if (!string.IsNullOrWhiteSpace(dest.ClientApplication.NativeDesktopMinimumConnectionSpeed))
                        {
                            dest.NativeDesktopApplication.Items.Add(
                                "Minimum connection speed",
                                new ListViewModel { Text = dest.ClientApplication.NativeDesktopMinimumConnectionSpeed });
                        }

                        if (!string.IsNullOrWhiteSpace(dest.ClientApplication.NativeDesktopMemoryAndStorage?.RecommendedResolution))
                        {
                            dest.NativeDesktopApplication.Items.Add(
                                "Screen resolution and aspect ratio",
                                new ListViewModel { Text = dest.ClientApplication.NativeDesktopMemoryAndStorage.RecommendedResolution });
                        }

                        if (!string.IsNullOrWhiteSpace(dest.ClientApplication.NativeDesktopMemoryAndStorage?.MinimumMemoryRequirement))
                        {
                            dest.NativeDesktopApplication.Items.Add(
                                "Memory size",
                                new ListViewModel { Text = dest.ClientApplication.NativeDesktopMemoryAndStorage.MinimumMemoryRequirement });
                        }

                        if (!string.IsNullOrWhiteSpace(dest.ClientApplication.NativeDesktopMemoryAndStorage?.StorageRequirementsDescription))
                        {
                            dest.NativeDesktopApplication.Items.Add(
                                "Storage space",
                                new ListViewModel { Text = dest.ClientApplication.NativeDesktopMemoryAndStorage.StorageRequirementsDescription });
                        }

                        if (!string.IsNullOrWhiteSpace(dest.ClientApplication.NativeDesktopMemoryAndStorage?.MinimumCpu))
                        {
                            dest.NativeDesktopApplication.Items.Add(
                                "Processing power",
                                new ListViewModel { Text = dest.ClientApplication.NativeDesktopMemoryAndStorage.MinimumCpu });
                        }

                        if (!string.IsNullOrWhiteSpace(dest.ClientApplication.NativeDesktopThirdParty?.ThirdPartyComponents))
                        {
                            dest.NativeDesktopApplication.Items.Add(
                                "Third-party components",
                                new ListViewModel { Text = dest.ClientApplication.NativeDesktopThirdParty.ThirdPartyComponents });
                        }

                        if (!string.IsNullOrWhiteSpace(dest.ClientApplication.NativeDesktopThirdParty?.DeviceCapabilities))
                        {
                            dest.NativeDesktopApplication.Items.Add(
                                "Device capabilities",
                                new ListViewModel { Text = dest.ClientApplication.NativeDesktopThirdParty.DeviceCapabilities });
                        }

                        if (!string.IsNullOrWhiteSpace(dest.ClientApplication.NativeDesktopHardwareRequirements))
                        {
                            dest.NativeDesktopApplication.Items.Add(
                                "Hardware requirements",
                                new ListViewModel { Text = dest.ClientApplication.NativeDesktopHardwareRequirements });
                        }

                        if (!string.IsNullOrWhiteSpace(dest.ClientApplication.NativeDesktopAdditionalInformation))
                        {
                            dest.NativeDesktopApplication.Items.Add(
                                "Additional information",
                                new ListViewModel { Text = dest.ClientApplication.NativeDesktopAdditionalInformation });
                        }

                        dest.NativeMobileApplication = new DescriptionListViewModel
                        {
                            Heading = "Mobile application",
                        };

                        if (dest.ClientApplication.MobileOperatingSystems?.OperatingSystems?.Any() == true)
                        {
                            dest.NativeMobileApplication.Items.Add(
                                "Supported operating systems",
                                new ListViewModel { List = dest.ClientApplication.MobileOperatingSystems.OperatingSystems.ToArray(), });
                        }

                        if (!string.IsNullOrWhiteSpace(dest.ClientApplication.MobileOperatingSystems?.OperatingSystemsDescription))
                        {
                            dest.NativeMobileApplication.Items.Add(
                                "Description of supported operating systems",
                                new ListViewModel { Text = dest.ClientApplication.MobileOperatingSystems.OperatingSystemsDescription });
                        }

                        if (!string.IsNullOrWhiteSpace(dest.ClientApplication.MobileConnectionDetails?.MinimumConnectionSpeed))
                        {
                            dest.NativeMobileApplication.Items.Add(
                                "Minimum connection speed",
                                new ListViewModel { Text = dest.ClientApplication.MobileConnectionDetails.MinimumConnectionSpeed });
                        }

                        if (dest.ClientApplication.MobileConnectionDetails?.ConnectionType?.Any() == true)
                        {
                            dest.NativeMobileApplication.Items.Add(
                                "Connection types supported",
                                new ListViewModel { List = dest.ClientApplication.MobileConnectionDetails.ConnectionType.ToArray(), });
                        }

                        if (!string.IsNullOrWhiteSpace(dest.ClientApplication.MobileConnectionDetails?.Description))
                        {
                            dest.NativeMobileApplication.Items.Add(
                                "Connection requirements",
                                new ListViewModel { Text = dest.ClientApplication.MobileConnectionDetails.Description });
                        }

                        if (!string.IsNullOrWhiteSpace(dest.ClientApplication.MobileMemoryAndStorage?.MinimumMemoryRequirement))
                        {
                            dest.NativeMobileApplication.Items.Add(
                                "Memory size",
                                new ListViewModel { Text = dest.ClientApplication.MobileMemoryAndStorage.MinimumMemoryRequirement });
                        }

                        if (!string.IsNullOrWhiteSpace(dest.ClientApplication.MobileMemoryAndStorage?.Description))
                        {
                            dest.NativeMobileApplication.Items.Add(
                                "Storage space",
                                new ListViewModel { Text = dest.ClientApplication.MobileMemoryAndStorage.Description });
                        }

                        if (!string.IsNullOrWhiteSpace(dest.ClientApplication.MobileThirdParty?.ThirdPartyComponents))
                        {
                            dest.NativeMobileApplication.Items.Add(
                                "Third-party components",
                                new ListViewModel { Text = dest.ClientApplication.MobileThirdParty.ThirdPartyComponents });
                        }

                        if (!string.IsNullOrWhiteSpace(dest.ClientApplication.MobileThirdParty?.DeviceCapabilities))
                        {
                            dest.NativeMobileApplication.Items.Add(
                                "Device capabilities",
                                new ListViewModel { Text = dest.ClientApplication.MobileThirdParty.DeviceCapabilities });
                        }

                        if (!string.IsNullOrWhiteSpace(dest.ClientApplication.NativeMobileHardwareRequirements))
                        {
                            dest.NativeMobileApplication.Items.Add(
                                "Hardware requirements",
                                new ListViewModel { Text = dest.ClientApplication.NativeMobileHardwareRequirements });
                        }

                        if (!string.IsNullOrWhiteSpace(dest.ClientApplication.NativeMobileAdditionalInformation))
                        {
                            dest.NativeMobileApplication.Items.Add(
                                "Additional information",
                                new ListViewModel { Text = dest.ClientApplication.NativeMobileAdditionalInformation });
                        }
                    });

            CreateMap<CatalogueItem, SolutionDisplayBaseModel>()
                .BeforeMap(
                    (src, dest) => dest.SetClientApplication(
                        src.Solution != null && !string.IsNullOrEmpty(src.Solution.ClientApplication)
                            ? JsonConvert.DeserializeObject<ClientApplication>(src.Solution.ClientApplication)
                            : new ClientApplication()))
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
                        (SectionModel previous, SectionModel next) = dest.PreviousAndNextModels();
                        dest.PaginationFooter = new PaginationFooterModel { Next = next, Previous = previous, };

                        if (!string.IsNullOrWhiteSpace(src.Solution?.FullDescription))
                        {
                            dest.SetShowTrue("Description");
                        }

                        if (!string.IsNullOrWhiteSpace(src.Solution?.Features))
                        {
                            dest.SetShowTrue("Features");
                        }

                        //if (!string.IsNullOrWhiteSpace(src.Solution?.SolutionCapabilities))
                        //{
                        //    dest.SetShowTrue("Capabilities");
                        //}

                        //if (!string.IsNullOrWhiteSpace(src.Solution?.ListPrice))
                        //{
                        //    dest.SetShowTrue("List price");
                        //}

                        //if (!string.IsNullOrWhiteSpace(src.Solution?.AssociatedServices))
                        //{
                        //    dest.SetShowTrue("Associated Services");
                        //}

                        //if (!string.IsNullOrWhiteSpace(src.Solution?.Interoperability))
                        //{
                        //    dest.SetShowTrue("Interoperability");
                        //}

                        if (!string.IsNullOrWhiteSpace(src.Solution?.ImplementationDetail))
                        {
                            dest.SetShowTrue("Implementation timescales");
                        }

                        //if (!string.IsNullOrWhiteSpace(src.Solution?.ClientApplication))
                        //{
                        //    dest.SetShowTrue("Client application type");
                        //}

                        //if (!string.IsNullOrWhiteSpace(src.Solution?.Hosting))
                        //{
                        //    dest.SetShowTrue("Hosting type");
                        //}

                        //if (!string.IsNullOrWhiteSpace(src.Solution?.ServiceLevelAgreement))
                        //{
                        //    dest.SetShowTrue("Service Level Agreement");
                        //}

                        //if (!string.IsNullOrWhiteSpace(src.Solution?.DevelopmentPlans))
                        //{
                        //    dest.SetShowTrue("Development plans");
                        //}

                        //if (!string.IsNullOrWhiteSpace(src.Solution?.SupplierDetails))
                        //{
                        //    dest.SetShowTrue("Supplier details");
                        //}
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
    }
}
