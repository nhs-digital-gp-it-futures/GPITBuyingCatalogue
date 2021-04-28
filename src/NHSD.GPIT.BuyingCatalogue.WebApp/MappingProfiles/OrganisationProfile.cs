using AutoMapper;
using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.AboutOrganisation;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.MappingProfiles
{
    public class OrganisationProfile : Profile
    {
        public OrganisationProfile()
        {
            CreateMap<CatalogueItem, AboutSupplierModel>()
                .ForMember(dest => dest.Description,
                    opt => opt.MapFrom(src => src.Supplier == null ? null : src.Supplier.Summary))
                .ForMember(dest => dest.Link,
                    opt => opt.MapFrom(src => src.Supplier == null ? null : src.Supplier.SupplierUrl))
                .IncludeBase<CatalogueItem, MarketingBaseModel>();
            
            CreateMap<CatalogueItem, ContactDetailsModel>()
                .ForMember(dest => dest.Contact1, opt => opt.MapFrom(src => src.FirstContact()))
                .ForMember(dest => dest.Contact2, opt => opt.MapFrom(src => src.SecondContact()))
                .IncludeBase<CatalogueItem, MarketingBaseModel>();
            
            CreateMap<ContactDetailsModel, SupplierContactsModel>()
                .ForMember(dest => dest.Contacts, opt => opt.MapFrom(src => new[] {src.Contact1, src.Contact2}))
                .ForMember(dest => dest.SolutionId, opt => opt.MapFrom(src => src.SolutionId));

            CreateMap<CatalogueItem, MarketingBaseModel>()
                .ForMember(dest => dest.BackLink,
                    opt => opt.MapFrom(src => $"/marketing/supplier/solution/{src.CatalogueItemId}"))
                .ForMember(dest => dest.BackLinkText, opt => opt.MapFrom(src => "Return to all sections"))
                .ForMember(dest => dest.ClientApplication,
                    opt => opt.MapFrom(src =>
                        src.Solution != null && !string.IsNullOrEmpty(src.Solution.ClientApplication)
                            ? JsonConvert.DeserializeObject<ClientApplication>(src.Solution.ClientApplication)
                            : new ClientApplication()))
                .ForMember(dest => dest.SolutionId, opt => opt.MapFrom(src => src.CatalogueItemId))
                .ForMember(dest => dest.SupplierId,
                    opt => opt.MapFrom(src => src.Supplier == null ? null : src.Supplier.Id))
                .IgnoreAllPropertiesWithAnInaccessibleSetter();
        }
    }
}