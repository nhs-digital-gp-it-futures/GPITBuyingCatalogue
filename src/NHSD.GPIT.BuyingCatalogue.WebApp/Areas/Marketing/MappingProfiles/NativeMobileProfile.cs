using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.NativeMobile;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.MappingProfiles
{
    public class NativeMobileProfile : Profile
    {
        public NativeMobileProfile()
        {
            CreateMap<CatalogueItem, AdditionalInformationModel>()
                .ForMember(dest => dest.BackLink, opt => opt.MapFrom(src => GetBackLink(src)))
                .ForMember(dest => dest.AdditionalInformation, opt =>
                {
                    opt.SetMappingOrder(20);
                    opt.MapFrom((_, dest) => dest.ClientApplication?.NativeMobileAdditionalInformation);
                })
                .IncludeBase<CatalogueItem, MarketingBaseModel>();

            CreateMap<CatalogueItem, ConnectivityModel>()
                .ForMember(dest => dest.BackLink, opt => opt.MapFrom(src => GetBackLink(src)))
                .ForMember(
                    dest => dest.ConnectionSpeeds,
                    opt => opt.MapFrom(src => new List<SelectListItem>(ProfileDefaults.ConnectionSpeeds)))
                .ForMember(
                    dest => dest.ConnectionTypes,
                    opt => opt.MapFrom(src => new List<ConnectionTypeModel>(ProfileDefaults.MobileConnectionTypes)))
                .ForMember(
                    dest => dest.Description,
                    opt =>
                    {
                        opt.SetMappingOrder(10);
                        opt.MapFrom((_, dest) => dest.ClientApplication?.MobileConnectionDetails?.Description);
                    })
                .ForMember(
                    dest => dest.SelectedConnectionSpeed,
                    opt =>
                    {
                        opt.SetMappingOrder(20);
                        opt.MapFrom(
                            (_, dest) => dest.ClientApplication?.MobileConnectionDetails?.MinimumConnectionSpeed);
                    })
                .IncludeBase<CatalogueItem, MarketingBaseModel>()
                .AfterMap(
                    (_, dest) =>
                    {
                        var mobilConnectionTypes = dest.ClientApplication?.MobileConnectionDetails?.ConnectionType;
                        if (mobilConnectionTypes == null)
                            return;

                        foreach (var connectionType in dest.ConnectionTypes)
                        {
                            connectionType.Checked = mobilConnectionTypes.Any(
                                x =>
                                    x.Equals(
                                        connectionType.ConnectionType,
                                        StringComparison.InvariantCultureIgnoreCase));
                        }
                    });

            CreateMap<CatalogueItem, HardwareRequirementsModel>()
                .ForMember(dest => dest.BackLink, opt => opt.MapFrom(src => GetBackLink(src)))
                .ForMember(dest => dest.Description, opt =>
                {
                    opt.SetMappingOrder(20);
                    opt.MapFrom((_, dest) => dest.ClientApplication?.NativeMobileHardwareRequirements);
                })
                .IncludeBase<CatalogueItem, MarketingBaseModel>();

            CreateMap<CatalogueItem, MemoryAndStorageModel>()
                .ForMember(dest => dest.BackLink, opt => opt.MapFrom(src => GetBackLink(src)))
                .ForMember(dest => dest.Description, opt =>
                {
                    opt.SetMappingOrder(10);
                    opt.MapFrom((_, dest) =>
                        dest.ClientApplication?.MobileMemoryAndStorage?.Description);
                })
                .ForMember(dest => dest.MemorySizes, opt => opt.MapFrom(src => ProfileDefaults.MemorySizes))
                .ForMember(dest => dest.SelectedMemorySize, opt =>
                {
                    opt.SetMappingOrder(20);
                    opt.MapFrom((_, dest) =>
                        dest.ClientApplication?.MobileMemoryAndStorage?.MinimumMemoryRequirement);
                })
                .IncludeBase<CatalogueItem, MarketingBaseModel>();

            CreateMap<CatalogueItem, MobileFirstApproachModel>()
                .ForMember(dest => dest.BackLink, opt => opt.MapFrom(src => GetBackLink(src)))
                .ForMember(dest => dest.MobileFirstApproach, opt =>
                {
                    opt.SetMappingOrder(20);
                    opt.MapFrom(
                        (_, dest) => dest.ClientApplication?.NativeMobileFirstDesign.ToYesNo());
                })
                .IncludeBase<CatalogueItem, MarketingBaseModel>();

            CreateMap<CatalogueItem, OperatingSystemsModel>()
                .ForMember(dest => dest.BackLink, opt => opt.MapFrom(src => GetBackLink(src)))
                .ForMember(dest => dest.Description, opt =>
                {
                    opt.SetMappingOrder(20);
                    opt.MapFrom(
                        (_, dest) => dest.ClientApplication?.MobileOperatingSystems?.OperatingSystemsDescription);
                })
                .ForMember(
                    dest => dest.OperatingSystems,
                    opt => opt.MapFrom(src => ProfileDefaults.SupportedOperatingSystems))
                .IncludeBase<CatalogueItem, MarketingBaseModel>()
                .AfterMap((_, dest) =>
                {
                    var operatingSystems = dest.ClientApplication?.MobileOperatingSystems?.OperatingSystems;
                    if (operatingSystems == null)
                        return;

                    foreach (var browser in dest.OperatingSystems)
                    {
                        browser.Checked = operatingSystems.Any(x =>
                            x.Equals(browser.OperatingSystemName, StringComparison.InvariantCultureIgnoreCase));
                    }
                });

            CreateMap<CatalogueItem, ThirdPartyModel>()
                .ForMember(dest => dest.BackLink, opt => opt.MapFrom(src => GetBackLink(src)))
                .ForMember(dest => dest.DeviceCapabilities, opt =>
                {
                    opt.SetMappingOrder(20);
                    opt.MapFrom((_, dest) => dest.ClientApplication?.MobileThirdParty?.DeviceCapabilities);
                })
                .ForMember(dest => dest.ThirdPartyComponents, opt =>
                {
                    opt.SetMappingOrder(30);
                    opt.MapFrom((_, dest) => dest.ClientApplication?.MobileThirdParty?.ThirdPartyComponents);
                })
                .IncludeBase<CatalogueItem, MarketingBaseModel>();

            CreateMap<ConnectivityModel, MobileConnectionDetails>()
                .ForMember(dest => dest.ConnectionType, opt => opt.MapFrom(src => new HashSet<string>()))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.MinimumConnectionSpeed, opt => opt.MapFrom(src => src.SelectedConnectionSpeed))
                .AfterMap((src, dest) =>
                {
                    if (src.ConnectionTypes == null || !src.ConnectionTypes.Any())
                        return;

                    foreach (var connectionType in src.ConnectionTypes.Where(x => x.Checked))
                        dest.ConnectionType.Add(connectionType.ConnectionType);
                });

            CreateMap<MemoryAndStorageModel, MobileMemoryAndStorage>()
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.MinimumMemoryRequirement, opt => opt.MapFrom(src => src.SelectedMemorySize));

            CreateMap<OperatingSystemsModel, MobileOperatingSystems>()
                .ForMember(dest => dest.OperatingSystems, opt => opt.MapFrom(src => new HashSet<string>()))
                .ForMember(dest => dest.OperatingSystemsDescription, opt => opt.MapFrom(src => src.Description))
                .AfterMap((src, dest) =>
                {
                    if (src.OperatingSystems == null || !src.OperatingSystems.Any())
                        return;

                    foreach (var operatingSystem in src.OperatingSystems.Where(x => x.Checked))
                        dest.OperatingSystems.Add(operatingSystem.OperatingSystemName);
                });

            CreateMap<ThirdPartyModel, MobileThirdParty>()
                .ForMember(dest => dest.DeviceCapabilities, opt => opt.MapFrom(src => src.DeviceCapabilities))
                .ForMember(dest => dest.ThirdPartyComponents, opt => opt.MapFrom(src => src.ThirdPartyComponents));
        }

        private static string GetBackLink(CatalogueItem catalogueItem) =>
            ProfileDefaults.GetNativeMobileBackLink(catalogueItem.CatalogueItemId);
    }
}
