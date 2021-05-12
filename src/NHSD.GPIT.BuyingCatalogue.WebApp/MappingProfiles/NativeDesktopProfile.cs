using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.NativeDesktop;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.MappingProfiles
{
    public class NativeDesktopProfile : Profile
    {
        public NativeDesktopProfile()
        {
            CreateMap<CatalogueItem, OperatingSystemsModel>()
                .ForMember(dest => dest.BackLink,
                    opt => opt.MapFrom(src => ProfileDefaults.GetNativeDesktopBackLink(src.CatalogueItemId)))
                .ForMember(dest => dest.OperatingSystemsDescription, opt =>
                {
                    opt.SetMappingOrder(10);
                    opt.MapFrom((_, dest) => dest.ClientApplication?.NativeDesktopOperatingSystemsDescription);
                })
                .IncludeBase<CatalogueItem, MarketingBaseModel>();

            CreateMap<CatalogueItem, ConnectivityModel>()
                    .ForMember(dest => dest.BackLink,
                    opt => opt.MapFrom(src => ProfileDefaults.GetNativeDesktopBackLink(src.CatalogueItemId)))
                .ForMember(dest => dest.ConnectionSpeeds,
                    opt => opt.MapFrom(src => ProfileDefaults.ConnectionSpeeds))
                .ForMember(dest => dest.SelectedConnectionSpeed, opt =>
                {
                    opt.SetMappingOrder(10);
                    opt.MapFrom((_, dest) => dest.ClientApplication?.NativeDesktopMinimumConnectionSpeed);
                })
                .IncludeBase<CatalogueItem, MarketingBaseModel>();

            CreateMap<CatalogueItem, MemoryAndStorageModel>()
                .ForMember(dest => dest.BackLink,
                    opt => opt.MapFrom(src => ProfileDefaults.GetNativeDesktopBackLink(src.CatalogueItemId)))
                .ForMember(dest => dest.MemorySizes,
                    opt => opt.MapFrom(src => ProfileDefaults.MemorySizes))
                .ForMember(dest => dest.ScreenResolutions,
                    opt => opt.MapFrom(src => ProfileDefaults.ScreenResolutions))
                .ForMember(dest => dest.SelectedScreenResolution, opt =>
                {
                    opt.SetMappingOrder(10);
                    opt.MapFrom((_, dest) => dest.ClientApplication?.NativeDesktopMemoryAndStorage?.RecommendedResolution);
                })
                .ForMember(dest => dest.SelectedMemorySize, opt =>
                {
                    opt.SetMappingOrder(20);
                    opt.MapFrom((_, dest) => dest.ClientApplication?.NativeDesktopMemoryAndStorage?.MinimumMemoryRequirement);
                })
                .ForMember(dest => dest.MinimumCpu, opt =>
                {
                    opt.SetMappingOrder(30);
                    opt.MapFrom((_, dest) => dest.ClientApplication?.NativeDesktopMemoryAndStorage?.MinimumCpu);
                })
                .ForMember(dest => dest.StorageDescription, opt =>
                {
                    opt.SetMappingOrder(40);
                    opt.MapFrom((_, dest) => dest.ClientApplication?.NativeDesktopMemoryAndStorage?.StorageRequirementsDescription);
                })
                .IncludeBase<CatalogueItem, MarketingBaseModel>();

            CreateMap<CatalogueItem, ThirdPartyModel>()
                .ForMember(dest => dest.BackLink,
                    opt => opt.MapFrom(src => ProfileDefaults.GetNativeDesktopBackLink(src.CatalogueItemId)))
                .ForMember(dest => dest.ThirdPartyComponents, opt =>
                {
                    opt.SetMappingOrder(10);
                    opt.MapFrom((_, dest) => dest.ClientApplication?.NativeDesktopThirdParty?.ThirdPartyComponents);
                })
                .ForMember(dest => dest.DeviceCapabilities, opt =>
                {
                    opt.SetMappingOrder(20);
                    opt.MapFrom((_, dest) => dest.ClientApplication?.NativeDesktopThirdParty?.DeviceCapabilities);
                })
                .IncludeBase<CatalogueItem, MarketingBaseModel>();

            CreateMap<CatalogueItem, HardwareRequirementsModel>()
                .ForMember(dest => dest.BackLink,
                    opt => opt.MapFrom(src => ProfileDefaults.GetNativeDesktopBackLink(src.CatalogueItemId)))
                .ForMember(dest => dest.Description, opt =>
                {
                    opt.SetMappingOrder(10);
                    opt.MapFrom((_, dest) => dest.ClientApplication?.NativeDesktopHardwareRequirements);
                })
                .IncludeBase<CatalogueItem, MarketingBaseModel>();

            CreateMap<CatalogueItem, AdditionalInformationModel>()
                .ForMember(dest => dest.BackLink,
                    opt => opt.MapFrom(src => ProfileDefaults.GetNativeDesktopBackLink(src.CatalogueItemId)))
                .ForMember(dest => dest.AdditionalInformation, opt =>
                {
                    opt.SetMappingOrder(10);
                    opt.MapFrom((_, dest) => dest.ClientApplication?.NativeDesktopAdditionalInformation);
                })
                .IncludeBase<CatalogueItem, MarketingBaseModel>();
        }
    }
}
