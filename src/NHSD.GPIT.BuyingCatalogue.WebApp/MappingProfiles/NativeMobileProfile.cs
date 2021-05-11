using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.NativeMobile;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.MappingProfiles
{
    public class NativeMobileProfile : Profile
    {
        public NativeMobileProfile()
        {
            CreateMap<CatalogueItem, ConnectivityModel>()
                .ForMember(dest => dest.ConnectionTypes,
                    opt => opt.MapFrom(src => new ConnectionTypeModel[]
                    {
                        new() { ConnectionType = "GPRS" },
                        new() { ConnectionType = "3G" },
                        new() { ConnectionType = "LTE" },
                        new() { ConnectionType = "4G" },
                        new() { ConnectionType = "5G" },
                        new() { ConnectionType = "Bluetooth" },
                        new() { ConnectionType = "Wifi" }
                    }))
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
                .ForMember(dest => dest.ConnectionSpeeds, opt => opt.MapFrom(src => new List<SelectListItem>
                {
                    new() { Text = "0.5Mbps", Value="0.5Mbps"},
                    new() { Text = "1Mbps", Value="1Mbps"},
                    new() { Text = "1.5Mbps", Value="1.5Mbps"},
                    new() { Text = "2Mbps", Value="2Mbps"},
                    new() { Text = "3Mbps", Value="3Mbps"},
                    new() { Text = "5Mbps", Value="5Mbps"},
                    new() { Text = "8Mbps", Value="8Mbps"},
                    new() { Text = "10Mbps", Value="10Mbps"},
                    new() { Text = "15Mbps", Value="15Mbps"},
                    new() { Text = "20Mbps", Value="20Mbps"},
                    new() { Text = "30Mbps", Value="30Mbps"},
                    new() { Text = "Higher than 30Mbps", Value="Higher than 30Mbps"},
                }))
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
                .ForMember(dest => dest.MemorySizes,
                    opt => opt.MapFrom(src => new List<SelectListItem>
                    {
                        new SelectListItem{ Text = "256MB", Value = "256MB"},
                        new SelectListItem{ Text = "512MB", Value = "512MB"},
                        new SelectListItem{ Text = "1GB", Value = "1GB"},
                        new SelectListItem{ Text = "2GB", Value = "2GB"},
                        new SelectListItem{ Text = "4GB", Value = "4GB"},
                        new SelectListItem{ Text = "8GB", Value = "8GB"},
                        new SelectListItem{ Text = "16GB or higher", Value = "16GB or higher"}
                    }))
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
                .ForMember(dest => dest.Description, opt =>
                {
                    opt.SetMappingOrder(10);
                    opt.MapFrom((_, dest) => dest.ClientApplication?.NativeMobileHardwareRequirements);
                })
                .IncludeBase<CatalogueItem, MarketingBaseModel>();

            CreateMap<CatalogueItem, AdditionalInformationModel>()
                .ForMember(dest => dest.AdditionalInformation, opt =>
                {
                    opt.SetMappingOrder(10);
                    opt.MapFrom((_, dest) => dest.ClientApplication?.NativeMobileAdditionalInformation);
                })
                .IncludeBase<CatalogueItem, MarketingBaseModel>();
        }
    }
}
