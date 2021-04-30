using System;
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
                                              x.Equals(browser.BrowserName,
                                                  StringComparison.InvariantCultureIgnoreCase));
                    }
                });

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