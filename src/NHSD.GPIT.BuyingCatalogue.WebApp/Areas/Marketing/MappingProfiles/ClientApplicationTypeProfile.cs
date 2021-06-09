using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.ClientApplicationType;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.MappingProfiles
{
    public class ClientApplicationTypeProfile : Profile
    {
        private const string KeyBrowserBased = "browser-based";
        private const string KeyNativeDesktop = "native-desktop";
        private const string KeyNativeMobile = "native-mobile";

        public ClientApplicationTypeProfile()
        {
            CreateMap<CatalogueItem, BrowserBasedModel>()
                .ForMember(
                    dest => dest.BackLink,
                    opt => opt.MapFrom(src => ProfileDefaults.GetSolutionBackLink(src.CatalogueItemId)))
                .IncludeBase<CatalogueItem, MarketingBaseModel>();

            CreateMap<CatalogueItem, ClientApplicationTypesModel>()
                .ForMember(
                    dest => dest.BackLink,
                    opt => opt.MapFrom(src => ProfileDefaults.GetSolutionBackLink(src.CatalogueItemId)))
                .ForMember(dest => dest.BrowserBased, opt =>
                {
                    opt.SetMappingOrder(20);
                    opt.MapFrom((_, dest) => dest.ClientApplication.ClientApplicationTypes.Any(x =>
                        x.EqualsIgnoreCase(KeyBrowserBased)));
                })
                .ForMember(dest => dest.NativeDesktop, opt =>
                {
                    opt.SetMappingOrder(21);
                    opt.MapFrom((_, dest) => dest.ClientApplication.ClientApplicationTypes.Any(x =>
                        x.EqualsIgnoreCase(KeyNativeDesktop)));
                })
                .ForMember(dest => dest.NativeMobile, opt =>
                {
                    opt.SetMappingOrder(22);
                    opt.MapFrom((_, dest) => dest.ClientApplication.ClientApplicationTypes.Any(x =>
                        x.EqualsIgnoreCase(KeyNativeMobile)));
                })
                .IncludeBase<CatalogueItem, MarketingBaseModel>();

            CreateMap<CatalogueItem, NativeDesktopModel>()
                .ForMember(
                    dest => dest.BackLink,
                    opt => opt.MapFrom(src => ProfileDefaults.GetSolutionBackLink(src.CatalogueItemId)))
                .IncludeBase<CatalogueItem, MarketingBaseModel>();

            CreateMap<CatalogueItem, NativeMobileModel>()
                .ForMember(
                    dest => dest.BackLink,
                    opt => opt.MapFrom(src => ProfileDefaults.GetSolutionBackLink(src.CatalogueItemId)))
                .IncludeBase<CatalogueItem, MarketingBaseModel>();

            CreateMap<ClientApplicationTypesModel, ClientApplication>()
                .ForMember(
                    dest => dest.ClientApplicationTypes,
                    opt => opt.MapFrom(src => GetClientApplicationTypes(src)))
                .ForAllOtherMembers(opt => opt.Ignore());
        }

        private static HashSet<string> GetClientApplicationTypes(ClientApplicationTypesModel model)
        {
            var result = new HashSet<string>();

            if (model.BrowserBased)
                result.Add(KeyBrowserBased);
            if (model.NativeDesktop)
                result.Add(KeyNativeDesktop);
            if (model.NativeMobile)
                result.Add(KeyNativeMobile);

            return result;
        }
    }
}
