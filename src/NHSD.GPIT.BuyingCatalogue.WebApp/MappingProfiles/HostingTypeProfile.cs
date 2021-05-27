using System;
using System.Linq;
using AutoMapper;
using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.HostingType;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.Solution;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.MappingProfiles
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

            CreateMap<CatalogueItem, MarketingDisplayBaseModel>()
                .ForMember(dest => dest.LastReviewed, opt => opt.Ignore())
                .ForMember(dest => dest.PaginationFooter, opt => opt.Ignore())
                .ForMember(dest => dest.Section, opt => opt.Ignore())
                .ForMember(dest => dest.SolutionId, opt => opt.MapFrom(src => src.CatalogueItemId))
                .IgnoreAllPropertiesWithAnInaccessibleSetter();

            CreateMap<CatalogueItem, SolutionStatusModel>()
                .ForMember(
                    dest => dest.Description,
                    opt => opt.MapFrom(src => src.Solution == null ? null : src.Solution.FullDescription))
                .ForMember(dest => dest.Framework, opt => opt.MapFrom(src => ProfileDefaults.Framework))
                .ForMember(dest => dest.LastReviewed, opt => opt.MapFrom(src => new DateTime(2020, 3, 31)))
                .ForMember(dest => dest.PaginationFooter, opt => opt.MapFrom(src => new PaginationFooterModel()))
                .ForMember(dest => dest.Section, opt => opt.MapFrom(src => "Description"))
                .ForMember(dest => dest.SolutionName, opt => opt.MapFrom(src => src.Name))
                .ForMember(
                    dest => dest.Summary,
                    opt => opt.MapFrom(src => src.Solution == null ? null : src.Solution.Summary))
                .ForMember(
                    dest => dest.SupplierName,
                    opt => opt.MapFrom(src => src.Supplier == null ? null : src.Supplier.Name))
                .IncludeBase<CatalogueItem, MarketingDisplayBaseModel>()
                .AfterMap(
                    (_, dest) =>
                    {
                        dest.PaginationFooter.Next = dest.GetSectionFor("Features");
                    });
        }
    }
}
