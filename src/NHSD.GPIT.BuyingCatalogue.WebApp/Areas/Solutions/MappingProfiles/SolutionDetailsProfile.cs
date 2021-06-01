using System;
using AutoMapper;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.MappingProfiles
{
    public class SolutionDetailsProfile : Profile
    {
        public SolutionDetailsProfile()
        {
            CreateMap<CatalogueItem, SolutionDisplayBaseModel>()
                .ForMember(
                    dest => dest.LastReviewed,
                    opt => opt.MapFrom<IMemberValueResolver<object, object, string, string>, string>(
                        x => "SolutionsLastReviewedDate"))
                .ForMember(dest => dest.PaginationFooter, opt => opt.MapFrom(src => new PaginationFooterModel()))
                .ForMember(dest => dest.Section, opt => opt.Ignore())
                .ForMember(dest => dest.SolutionId, opt => opt.MapFrom(src => src.CatalogueItemId))
                .ForMember(dest => dest.SolutionName, opt => opt.MapFrom(src => src.Name))
                .IgnoreAllPropertiesWithAnInaccessibleSetter()
                .AfterMap(
                    (_, dest) =>
                    {
                        (SectionModel previous, SectionModel next) = dest.PreviousAndNextModels();
                        dest.PaginationFooter.Next = next;
                        dest.PaginationFooter.Previous = previous;
                    });

            CreateMap<CatalogueItem, ImplementationTimescalesModel>()
                .ForMember(
                    dest => dest.Description,
                    opt => opt.MapFrom(src => src.Solution == null ? null : src.Solution.ImplementationDetail))
                .ForMember(dest => dest.Section, opt => opt.MapFrom(src => "Implementation timescales"))
                .IncludeBase<CatalogueItem, SolutionDisplayBaseModel>();

            CreateMap<CatalogueItem, SolutionDescriptionModel>()
                .ForMember(
                    dest => dest.Description,
                    opt => opt.MapFrom(src => src.Solution == null ? null : src.Solution.FullDescription))
                .ForMember(dest => dest.Framework, opt => opt.MapFrom(src => src.Framework()))
                .ForMember(dest => dest.IsFoundation, opt => opt.MapFrom(src => src.IsFoundation().ToYesNo()))
                .ForMember(dest => dest.Section, opt => opt.MapFrom(src => "Description"))
                .ForMember(
                    dest => dest.Summary,
                    opt => opt.MapFrom(src => src.Solution == null ? null : src.Solution.Summary))
                .ForMember(
                    dest => dest.SupplierName,
                    opt => opt.MapFrom(src => src.Supplier == null ? null : src.Supplier.Name))
                .IncludeBase<CatalogueItem, SolutionDisplayBaseModel>();

            CreateMap<CatalogueItem, SolutionFeaturesModel>()
                .ForMember(dest => dest.Features, opt => opt.MapFrom(src => src.Features()))
                .ForMember(dest => dest.Section, opt => opt.MapFrom(src => "Features"))
                .IncludeBase<CatalogueItem, SolutionDisplayBaseModel>()
                .AfterMap(
                    (_, dest) =>
                    {
                        dest.PaginationFooter.Previous = dest.GetSectionFor("Description");
                        dest.PaginationFooter.Next = dest.GetSectionFor("Capabilities");
                    });
        }
    }
}
