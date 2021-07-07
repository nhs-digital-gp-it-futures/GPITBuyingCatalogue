using AutoMapper;
using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.HostingType;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.Solution;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.MappingProfiles
{
    public class HostingTypeProfile : Profile
    {
        public HostingTypeProfile()
        {
            CreateMap<CatalogueItem, HybridModel>()
                .ForMember(dest => dest.BackLink, opt => opt.MapFrom(src => ProfileDefaults.GetSolutionBackLink(src.CatalogueItemId)))
                .ForMember(
                    dest => dest.HybridHostingType,
                    opt => opt.MapFrom(src =>
                        src.Solution == null ? new HybridHostingType() :
                        string.IsNullOrWhiteSpace(src.Solution.Hosting) ? new HybridHostingType() :
                        JsonConvert.DeserializeObject<Hosting>(src.Solution.Hosting).HybridHostingType))
                .ForMember(dest => dest.RequiresHscnChecked, opt => opt.Ignore())
                .IncludeBase<CatalogueItem, MarketingBaseModel>();

            CreateMap<CatalogueItem, OnPremiseModel>()
                .ForMember(dest => dest.BackLink, opt => opt.MapFrom(src => ProfileDefaults.GetSolutionBackLink(src.CatalogueItemId)))
                .ForMember(
                    dest => dest.OnPremise,
                    opt => opt.MapFrom(src =>
                        src.Solution == null ? new OnPremise() :
                        string.IsNullOrWhiteSpace(src.Solution.Hosting) ? new OnPremise() :
                        JsonConvert.DeserializeObject<Hosting>(src.Solution.Hosting).OnPremise))
                .ForMember(dest => dest.RequiresHscnChecked, opt => opt.Ignore())
                .IncludeBase<CatalogueItem, MarketingBaseModel>();

            CreateMap<CatalogueItem, PrivateCloudModel>()
                .ForMember(dest => dest.BackLink, opt => opt.MapFrom(src => ProfileDefaults.GetSolutionBackLink(src.CatalogueItemId)))
                .ForMember(
                    dest => dest.PrivateCloud,
                    opt => opt.MapFrom(src =>
                        src.Solution == null ? new PrivateCloud() :
                        string.IsNullOrWhiteSpace(src.Solution.Hosting) ? new PrivateCloud() :
                        JsonConvert.DeserializeObject<Hosting>(src.Solution.Hosting).PrivateCloud))
                .ForMember(dest => dest.RequiresHscnChecked, opt => opt.Ignore())
                .IncludeBase<CatalogueItem, MarketingBaseModel>();

            CreateMap<CatalogueItem, PublicCloudModel>()
                .ForMember(dest => dest.BackLink, opt => opt.MapFrom(src => ProfileDefaults.GetSolutionBackLink(src.CatalogueItemId)))
                .ForMember(
                    dest => dest.PublicCloud,
                    opt => opt.MapFrom(src =>
                        src.Solution == null ? new PublicCloud() :
                        string.IsNullOrWhiteSpace(src.Solution.Hosting) ? new PublicCloud() :
                        JsonConvert.DeserializeObject<Hosting>(src.Solution.Hosting).PublicCloud))
                .ForMember(dest => dest.RequiresHscnChecked, opt => opt.Ignore())
                .IncludeBase<CatalogueItem, MarketingBaseModel>();

            CreateMap<CatalogueItem, SolutionStatusModel>()
                .ConvertUsing<ITypeConverter<CatalogueItem, SolutionStatusModel>>();
        }
    }
}
