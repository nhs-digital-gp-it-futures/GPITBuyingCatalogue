using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.BrowserBased;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.MappingProfiles
{
    public class BrowserBasedProfile : Profile
    {
        public BrowserBasedProfile()
        {
            CreateMap<CatalogueItem, AdditionalInformationModel>()
                .ForMember(dest => dest.AdditionalInformation, opt =>
                {
                    opt.SetMappingOrder(20);
                    opt.MapFrom((_, dest) => dest.ClientApplication?.AdditionalInformation);
                })
                .ForMember(
                    dest => dest.BackLink,
                    opt => opt.MapFrom(src => ProfileDefaults.GetBrowserBasedBackLink(src.CatalogueItemId)))
                .IncludeBase<CatalogueItem, MarketingBaseModel>();

            CreateMap<CatalogueItem, ConnectivityAndResolutionModel>()
                .ForMember(
                    dest => dest.BackLink,
                    opt => opt.MapFrom(src => ProfileDefaults.GetBrowserBasedBackLink(src.CatalogueItemId)))
                .ForMember(dest => dest.ConnectionSpeeds, opt => opt.MapFrom(src => ProfileDefaults.ConnectionSpeeds))
                .ForMember(dest => dest.ScreenResolutions, opt => opt.MapFrom(src => ProfileDefaults.ScreenResolutions))
                .ForMember(dest => dest.SelectedConnectionSpeed, opt => opt.Ignore())
                .ForMember(dest => dest.SelectedScreenResolution, opt => opt.Ignore())
                .IncludeBase<CatalogueItem, MarketingBaseModel>()
                .AfterMap((_, dest) =>
                {
                    if (dest.ClientApplication != null)
                    {
                        dest.SelectedConnectionSpeed = dest.ClientApplication.MinimumConnectionSpeed;
                        dest.SelectedScreenResolution = dest.ClientApplication.MinimumDesktopResolution;
                    }
                });

            CreateMap<CatalogueItem, HardwareRequirementsModel>()
                .ForMember(
                    dest => dest.BackLink,
                    opt => opt.MapFrom(src => ProfileDefaults.GetBrowserBasedBackLink(src.CatalogueItemId)))
                .ForMember(dest => dest.Description, opt =>
                {
                    opt.SetMappingOrder(20);
                    opt.MapFrom((_, dest) => dest.ClientApplication?.HardwareRequirements);
                })
                .IncludeBase<CatalogueItem, MarketingBaseModel>();

            CreateMap<CatalogueItem, MobileFirstApproachModel>()
                .ForMember(
                    dest => dest.BackLink,
                    opt => opt.MapFrom(src => ProfileDefaults.GetBrowserBasedBackLink(src.CatalogueItemId)))
                .ForMember(dest => dest.MobileFirstApproach, opt =>
                {
                    opt.SetMappingOrder(20);
                    opt.Condition((_, dest) => dest.ClientApplication.MobileFirstDesign.HasValue);
                    opt.MapFrom((_, dest) => dest.ClientApplication.MobileFirstDesign.ToYesNo());
                })
                .IncludeBase<CatalogueItem, MarketingBaseModel>();

            CreateMap<CatalogueItem, PlugInsOrExtensionsModel>()
                .ForMember(dest => dest.AdditionalInformation, opt => opt.Ignore())
                .ForMember(
                    dest => dest.BackLink,
                    opt => opt.MapFrom(src => ProfileDefaults.GetBrowserBasedBackLink(src.CatalogueItemId)))
                .ForMember(dest => dest.PlugInsRequired, opt => opt.Ignore())
                .IncludeBase<CatalogueItem, MarketingBaseModel>()
                .AfterMap((_, dest) =>
                {
                    if (dest.ClientApplication.Plugins is { Required: { } } plugins)
                    {
                        dest.PlugInsRequired = plugins.Required.ToYesNo();
                        dest.AdditionalInformation = plugins.AdditionalInformation;
                    }
                });

            CreateMap<CatalogueItem, SupportedBrowsersModel>()
                .ForMember(
                    dest => dest.BackLink,
                    opt => opt.MapFrom(src => ProfileDefaults.GetBrowserBasedBackLink(src.CatalogueItemId)))
                .ForMember(dest => dest.Browsers, opt => opt.MapFrom(src => ProfileDefaults.SupportedBrowsers))
                .ForMember(dest => dest.MobileResponsive, opt => opt.Ignore())
                .IncludeBase<CatalogueItem, MarketingBaseModel>()
                .AfterMap((_, dest) =>
                {
                    dest.MobileResponsive = dest.ClientApplication.MobileResponsive.ToYesNo();
                    foreach (var browser in dest.Browsers)
                    {
                        browser.Checked = dest.ClientApplication.BrowsersSupported != null &&
                                          dest.ClientApplication.BrowsersSupported.Any(x =>
                                              x.EqualsIgnoreCase(browser.BrowserName));
                    }
                });

            CreateMap<ConnectivityAndResolutionModel, ClientApplication>()
                .ForMember(dest => dest.MinimumConnectionSpeed, opt => opt.MapFrom(src => src.SelectedConnectionSpeed))
                .ForMember(
                    dest => dest.MinimumDesktopResolution,
                    opt => opt.MapFrom(src => src.SelectedScreenResolution))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<PlugInsOrExtensionsModel, Plugins>()
                .ForMember(dest => dest.AdditionalInformation, opt => opt.MapFrom(src => src.AdditionalInformation))
                .ForMember(dest => dest.Required, opt =>
                    opt.MapFrom<IMemberValueResolver<object, object, string, bool?>, string>(x =>
                        x.PlugInsRequired));

            CreateMap<string, bool?>()
                .ConvertUsing<StringToNullableBoolResolver>();

            CreateMap<SupportedBrowsersModel, ClientApplication>()
                .BeforeMap((_, dest) => dest.BrowsersSupported.Clear())
                .ForMember(
                    dest => dest.BrowsersSupported,
                    opt => opt.MapFrom(src =>
                        src.Browsers == null
                            ? new HashSet<string>()
                            : src.Browsers.Where(x => x.Checked).Select(x => x.BrowserName)))
                .ForMember(
                    dest => dest.MobileResponsive,
                    opt => opt.MapFrom<IMemberValueResolver<object, object, string, bool?>, string>(x =>
                        x.MobileResponsive))
                .ForAllOtherMembers(opt => opt.Ignore());
        }
    }
}
