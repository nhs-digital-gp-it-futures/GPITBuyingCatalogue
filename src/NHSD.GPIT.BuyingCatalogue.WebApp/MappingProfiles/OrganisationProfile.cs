using AutoMapper;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.AboutOrganisation;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.MappingProfiles
{
    public class OrganisationProfile : Profile
    {
        public OrganisationProfile()
        {
            CreateMap<ContactDetailsModel, SupplierContactsModel>()
                .ForMember(dest => dest.Contacts, opt => opt.MapFrom(src => new[] {src.Contact1, src.Contact2}))
                .ForMember(dest => dest.SolutionId, opt => opt.MapFrom(src => src.SolutionId));
        }
    }
}