using System.Collections.Generic;
using AutoMapper;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.MappingProfiles
{
    public class CatalogueSolutionsProfile : Profile
    {
        public CatalogueSolutionsProfile()
        {
            CreateMap<IList<CatalogueItem>, CatalogueSolutionsModel>()
                .ForMember(dest => dest.AllPublicationStatuses, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.CatalogueItems, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.PublicationStatusId, opt => opt.Ignore());
        }
    }
}
