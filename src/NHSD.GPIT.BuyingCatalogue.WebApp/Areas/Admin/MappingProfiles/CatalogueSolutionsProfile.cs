using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.MappingProfiles
{
    public class CatalogueSolutionsProfile : Profile
    {
        public CatalogueSolutionsProfile()
        {
            CreateMap<CatalogueItem, CatalogueModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.CatalogueItemId.ToString()))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(
                    dest => dest.LastUpdated,
                    opt => opt.MapFrom(src => src.Supplier == null ? DateTime.MinValue : src.Supplier.LastUpdated))
                .ForMember(dest => dest.PublishedStatus, opt => opt.MapFrom(src => GetStatus(src.PublishedStatus)))
                .ForMember(dest => dest.PublishedStatusId, opt => opt.MapFrom(src => src.PublishedStatus))
                .ForMember(
                    dest => dest.Supplier,
                    opt => opt.MapFrom(src => src.Supplier == null ? string.Empty : src.Supplier.Name));

            CreateMap<IList<CatalogueItem>, CatalogueSolutionsModel>()
                .ForMember(
                    dest => dest.AllPublicationStatuses,
                    opt => opt.MapFrom(src => Enum.GetValues<PublicationStatus>()))
                .ForMember(dest => dest.CatalogueItems, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.PublicationStatusId, opt => opt.Ignore());

            CreateMap<PublicationStatus, PublicationStatusModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Checked, opt => opt.Ignore())
                .ForMember(dest => dest.Display, opt => opt.MapFrom(src => GetStatus(src)));
        }

        private static string GetStatus(PublicationStatus publicationStatus) =>
            publicationStatus switch
            {
                PublicationStatus.Draft => "Suspended",
                PublicationStatus.Withdrawn => "Deleted",
                _ => publicationStatus.ToString(),
            };
    }
}
