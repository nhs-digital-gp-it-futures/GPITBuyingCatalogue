using System.Linq;
using AutoMapper;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.MappingProfiles
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
                .IgnoreAllPropertiesWithAnInaccessibleSetter();

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
                .IncludeBase<CatalogueItem, SolutionDisplayBaseModel>()
                .AfterMap(
                    (_, dest) =>
                    {
                        dest.PaginationFooter.Next = dest.GetSectionFor("Features");
                    });
        }
    }
}
