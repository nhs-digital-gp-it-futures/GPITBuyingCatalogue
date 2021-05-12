using System;
using System.Linq;
using AutoMapper;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.NativeMobile;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.MappingProfiles
{
    public class NativeMobileProfile : Profile
    {
        public NativeMobileProfile()
        {
            CreateMap<CatalogueItem, OperatingSystemsModel>()
                .ForMember(dest => dest.BackLink,
                    opt => opt.MapFrom(src => ProfileDefaults.GetNativeMobileBackLink(src.CatalogueItemId)))
                .ForMember(dest => dest.Description, opt =>
                {
                    opt.SetMappingOrder(10);
                    opt.MapFrom((_, dest) => dest.ClientApplication?.MobileOperatingSystems?.OperatingSystemsDescription);
                })
                .ForMember(dest => dest.OperatingSystems,
                    opt => opt.MapFrom(src => ProfileDefaults.SupportedOperatingSystems))
                .IncludeBase<CatalogueItem, MarketingBaseModel>()
                .AfterMap((_, dest) =>
                {
                    foreach (var browser in dest.OperatingSystems)
                    {
                        if (dest.ClientApplication?.MobileOperatingSystems?.OperatingSystems != null &&
                            dest.ClientApplication.MobileOperatingSystems.OperatingSystems.Any(x => x.Equals(browser.OperatingSystemName, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            browser.Checked = true;
                        }
                    }
                });

            CreateMap<CatalogueItem, MobileFirstApproachModel>()
                .ForMember(dest => dest.BackLink,
                    opt => opt.MapFrom(src => ProfileDefaults.GetNativeMobileBackLink(src.CatalogueItemId)))
                .ForMember(dest => dest.MobileFirstApproach, opt =>
                {
                    opt.SetMappingOrder(10);
                    opt.Condition((_, dest) => dest.ClientApplication.NativeMobileFirstDesign.HasValue);
                    opt.MapFrom((_, dest) => dest.ClientApplication.NativeMobileFirstDesign.ToYesNo());
                })
                .IncludeBase<CatalogueItem, MarketingBaseModel>();

            CreateMap<CatalogueItem, ConnectivityModel>()
                .ForMember(dest => dest.BackLink,
                    opt => opt.MapFrom(src => ProfileDefaults.GetNativeMobileBackLink(src.CatalogueItemId)))
                .ForMember(dest => dest.ConnectionTypes,
                    opt => opt.MapFrom(src => ProfileDefaults.MobileConnectionTypes))
                .ForMember(dest => dest.Description, opt =>
                {
                    opt.SetMappingOrder(10);
                    opt.MapFrom((_, dest) => dest.ClientApplication?.MobileConnectionDetails?.Description);
                })
                .ForMember(dest => dest.SelectedConnectionSpeed, opt =>
                {
                    opt.SetMappingOrder(20);
                    opt.MapFrom((_, dest) => dest.ClientApplication?.MobileConnectionDetails?.MinimumConnectionSpeed);
                })
                .ForMember(dest => dest.ConnectionSpeeds,
                    opt => opt.MapFrom(src => ProfileDefaults.ConnectionSpeeds))
                .IncludeBase<CatalogueItem, MarketingBaseModel>()
                .AfterMap((_, dest) =>
                {
                    foreach (var connectionType in dest.ConnectionTypes)
                    {
                        if (dest.ClientApplication?.MobileConnectionDetails?.ConnectionType != null &&
                            dest.ClientApplication.MobileConnectionDetails.ConnectionType.Any(x => x.Equals(connectionType.ConnectionType, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            connectionType.Checked = true;
                        }
                    }
                });

            CreateMap<CatalogueItem, MemoryAndStorageModel>()
                .ForMember(dest => dest.BackLink,
                    opt => opt.MapFrom(src => ProfileDefaults.GetNativeMobileBackLink(src.CatalogueItemId)))
                .ForMember(dest => dest.MemorySizes,
                    opt => opt.MapFrom(src => ProfileDefaults.MemorySizes))
                .ForMember(dest => dest.SelectedMemorySize, opt =>
                {
                    opt.SetMappingOrder(10);
                    opt.MapFrom((_, dest) => dest.ClientApplication?.MobileMemoryAndStorage?.MinimumMemoryRequirement);
                })
                .ForMember(dest => dest.Description, opt =>
                {
                    opt.SetMappingOrder(20);
                    opt.MapFrom((_, dest) => dest.ClientApplication?.MobileMemoryAndStorage?.Description);
                })
                .IncludeBase<CatalogueItem, MarketingBaseModel>();

            CreateMap<CatalogueItem, ThirdPartyModel>()
                .ForMember(dest => dest.BackLink,
                    opt => opt.MapFrom(src => ProfileDefaults.GetNativeMobileBackLink(src.CatalogueItemId)))
                .ForMember(dest => dest.ThirdPartyComponents, opt =>
                {
                    opt.SetMappingOrder(10);
                    opt.MapFrom((_, dest) => dest.ClientApplication?.MobileThirdParty?.ThirdPartyComponents);
                })
                .ForMember(dest => dest.DeviceCapabilities, opt =>
                {
                    opt.SetMappingOrder(20);
                    opt.MapFrom((_, dest) => dest.ClientApplication?.MobileThirdParty?.DeviceCapabilities);
                })
                .IncludeBase<CatalogueItem, MarketingBaseModel>();

            CreateMap<CatalogueItem, HardwareRequirementsModel>()
                .ForMember(dest => dest.BackLink,
                    opt => opt.MapFrom(src => ProfileDefaults.GetNativeMobileBackLink(src.CatalogueItemId)))
                .ForMember(dest => dest.Description, opt =>
                {
                    opt.SetMappingOrder(10);
                    opt.MapFrom((_, dest) => dest.ClientApplication?.NativeMobileHardwareRequirements);
                })
                .IncludeBase<CatalogueItem, MarketingBaseModel>();

            CreateMap<CatalogueItem, AdditionalInformationModel>()
                .ForMember(dest => dest.BackLink,
                    opt => opt.MapFrom(src => ProfileDefaults.GetNativeMobileBackLink(src.CatalogueItemId)))
                .ForMember(dest => dest.AdditionalInformation, opt =>
                {
                    opt.SetMappingOrder(10);
                    opt.MapFrom((_, dest) => dest.ClientApplication?.NativeMobileAdditionalInformation);
                })
                .IncludeBase<CatalogueItem, MarketingBaseModel>();
        }
    }
}
