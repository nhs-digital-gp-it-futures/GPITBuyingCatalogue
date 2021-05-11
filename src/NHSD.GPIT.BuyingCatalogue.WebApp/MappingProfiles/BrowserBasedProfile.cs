using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
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
                .ForMember(dest => dest.BackLink,
                    opt => opt.MapFrom(src =>
                        $"/marketing/supplier/solution/{src.CatalogueItemId}/section/browser-based"))
                .IncludeBase<CatalogueItem, MarketingBaseModel>();

            CreateMap<CatalogueItem, ConnectivityAndResolutionModel>()
                .ForMember(dest => dest.BackLink,
                    opt => opt.MapFrom(src =>
                        $"/marketing/supplier/solution/{src.CatalogueItemId}/section/browser-based"))
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
                .ForMember(dest => dest.ScreenResolutions, opt => opt.MapFrom(src => new List<SelectListItem>
                {
                    new() { Text = "16:9 - 640 x 360", Value = "16:9 - 640 x 360" },
                    new() { Text = "4:3 - 800 x 600", Value = "4:3 - 800 x 600" },
                    new() { Text = "4:3 - 1024 x 768", Value = "4:3 - 1024 x 768" },
                    new() { Text = "16:9 - 1280 x 720", Value = "16:9 - 1280 x 720" },
                    new() { Text = "16:10 - 1280 x 800", Value = "16:10 - 1280 x 800" },
                    new() { Text = "5:4 - 1280 x 1024", Value = "5:4 - 1280 x 1024" },
                    new() { Text = "16:9 - 1360 x 768", Value = "16:9 - 1360 x 768" },
                    new() { Text = "16:9 - 1366 x 768", Value = "16:9 - 1366 x 768" },
                    new() { Text = "16:10 - 1440 x 900", Value = "16:10 - 1440 x 900" },
                    new() { Text = "16:9 - 1536 x 864", Value = "16:9 - 1536 x 864" },
                    new() { Text = "16:9 - 1600 x 900", Value = "16:9 - 1600 x 900" },
                    new() { Text = "16:10 - 1680 x 1050", Value = "16:10 - 1680 x 1050" },
                    new() { Text = "16:9 - 1920 x 1080", Value = "16:9 - 1920 x 1080" },
                    new() { Text = "16:10 - 1920 x 1200", Value = "16:10 - 1920 x 1200" },
                    new() { Text = "16:9 - 2048 x 1152", Value = "16:9 - 2048 x 1152" },
                    new() { Text = "21:9 - 2560 x 1080", Value = "21:9 - 2560 x 1080" },
                    new() { Text = "16:9 - 2560 x 1440", Value = "16:9 - 2560 x 1440" },
                    new() { Text = "21:9 - 3440 x 1440", Value = "21:9 - 3440 x 1440" },
                    new() { Text = "16:9 - 3840 x 2160", Value = "16:9 - 3840 x 2160" }                
                }))
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
                .ForMember(dest => dest.BackLink,
                    opt => opt.MapFrom(src =>
                        $"/marketing/supplier/solution/{src.CatalogueItemId}/section/browser-based"))
                .ForMember(dest => dest.Description, opt =>
                {
                    opt.SetMappingOrder(20);
                    opt.MapFrom((_, dest) => dest.ClientApplication?.HardwareRequirements);
                })
                .IncludeBase<CatalogueItem, MarketingBaseModel>();
                
            CreateMap<CatalogueItem, MobileFirstApproachModel>()
                .ForMember(dest => dest.BackLink,
                    opt => opt.MapFrom(src =>
                        $"/marketing/supplier/solution/{src.CatalogueItemId}/section/browser-based"))
                .ForMember(dest => dest.MobileFirstApproach, opt =>
                {
                    opt.SetMappingOrder(20);
                    opt.Condition((_, dest) => dest.ClientApplication.MobileFirstDesign.HasValue);
                    opt.MapFrom((_, dest) => dest.ClientApplication.MobileFirstDesign.ToYesNo());
                })
                .IncludeBase<CatalogueItem, MarketingBaseModel>();

            CreateMap<CatalogueItem, PlugInsOrExtensionsModel>()
                .ForMember(dest => dest.AdditionalInformation, opt => opt.Ignore())
                .ForMember(dest => dest.BackLink,
                    opt => opt.MapFrom(src =>
                        $"/marketing/supplier/solution/{src.CatalogueItemId}/section/browser-based"))
                .ForMember(dest => dest.PlugInsRequired, opt => opt.Ignore())
                .IncludeBase<CatalogueItem, MarketingBaseModel>()
                .AfterMap((_, dest) =>
                {
                    if (dest.ClientApplication.Plugins is {Required: { }} plugins)
                    {
                        dest.PlugInsRequired = plugins.Required.ToYesNo();
                        dest.AdditionalInformation = plugins.AdditionalInformation;
                    }
                });

            CreateMap<CatalogueItem, SupportedBrowsersModel>()
                .ForMember(dest => dest.BackLink,
                    opt => opt.MapFrom(src =>
                        $"/marketing/supplier/solution/{src.CatalogueItemId}/section/browser-based"))
                .ForMember(dest => dest.Browsers, opt => opt.MapFrom(src => new SupportedBrowserModel[]
                {
                    new() {BrowserName = "Google Chrome"},
                    new() {BrowserName = "Microsoft Edge"},
                    new() {BrowserName = "Mozilla Firefox"},
                    new() {BrowserName = "Opera"},
                    new() {BrowserName = "Safari"},
                    new() {BrowserName = "Chromium"},
                    new() {BrowserName = "Internet Explorer 11"},
                    new() {BrowserName = "Internet Explorer 10"}
                }))
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
                .ForMember(dest => dest.MinimumDesktopResolution,
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
                .ForMember(dest => dest.BrowsersSupported,
                    opt => opt.MapFrom(src =>
                        src.Browsers == null
                            ? new HashSet<string>()
                            : src.Browsers.Where(x => x.Checked).Select(x => x.BrowserName)))
                .ForMember(dest => dest.MobileResponsive,
                    opt => opt.MapFrom<IMemberValueResolver<object, object, string, bool?>, string>(x =>
                        x.MobileResponsive))
                .ForAllOtherMembers(opt => opt.Ignore());
        }
    }
}
