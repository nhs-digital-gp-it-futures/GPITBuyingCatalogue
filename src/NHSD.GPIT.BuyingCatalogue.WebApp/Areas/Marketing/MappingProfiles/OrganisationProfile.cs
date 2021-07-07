using System.Collections.Generic;
using AutoMapper;
using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.AboutOrganisation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.AboutSolution;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.MappingProfiles
{
    public class OrganisationProfile : Profile
    {
        public OrganisationProfile()
        {
            CreateMap<CatalogueItem, AboutSupplierModel>()
                .ForMember(
                    dest => dest.Description,
                    opt => opt.MapFrom(src => src.Supplier == null ? null : src.Supplier.Summary))
                .ForMember(
                    dest => dest.Link,
                    opt => opt.MapFrom(src => src.Supplier == null ? null : src.Supplier.SupplierUrl))
                .IncludeBase<CatalogueItem, MarketingBaseModel>();

            CreateMap<CatalogueItem, ContactDetailsModel>()
                .ForMember(dest => dest.Contact1, opt => opt.MapFrom(src => src.FirstContact()))
                .ForMember(dest => dest.Contact2, opt => opt.MapFrom(src => src.SecondContact()))
                .IncludeBase<CatalogueItem, MarketingBaseModel>();

            CreateMap<CatalogueItem, FeaturesModel>()
                .ForMember(dest => dest.Listing1, opt => opt.Ignore())
                .ForMember(dest => dest.Listing2, opt => opt.Ignore())
                .ForMember(dest => dest.Listing3, opt => opt.Ignore())
                .ForMember(dest => dest.Listing4, opt => opt.Ignore())
                .ForMember(dest => dest.Listing5, opt => opt.Ignore())
                .ForMember(dest => dest.Listing6, opt => opt.Ignore())
                .ForMember(dest => dest.Listing7, opt => opt.Ignore())
                .ForMember(dest => dest.Listing8, opt => opt.Ignore())
                .ForMember(dest => dest.Listing9, opt => opt.Ignore())
                .ForMember(dest => dest.Listing10, opt => opt.Ignore())
                .IncludeBase<CatalogueItem, MarketingBaseModel>()
                .AfterMap((src, dest) =>
                {
                    if (string.IsNullOrWhiteSpace(src.Solution.Features))
                        return;

                    var features = JsonConvert.DeserializeObject<string[]>(src.Solution.Features);
                    dest.Listing1 = features.Length > 0 ? features[0] : string.Empty;
                    dest.Listing2 = features.Length > 1 ? features[1] : string.Empty;
                    dest.Listing3 = features.Length > 2 ? features[2] : string.Empty;
                    dest.Listing4 = features.Length > 3 ? features[3] : string.Empty;
                    dest.Listing5 = features.Length > 4 ? features[4] : string.Empty;
                    dest.Listing6 = features.Length > 5 ? features[5] : string.Empty;
                    dest.Listing7 = features.Length > 6 ? features[6] : string.Empty;
                    dest.Listing8 = features.Length > 7 ? features[7] : string.Empty;
                    dest.Listing9 = features.Length > 8 ? features[8] : string.Empty;
                    dest.Listing10 = features.Length > 9 ? features[9] : string.Empty;
                });

            CreateMap<CatalogueItem, ImplementationTimescalesModel>()
                .ForMember(
                    dest => dest.Description,
                    opt => opt.MapFrom(src => src.Solution == null ? null : src.Solution.ImplementationDetail))
                .IncludeBase<CatalogueItem, MarketingBaseModel>();

            CreateMap<CatalogueItem, IntegrationsModel>()
                .ForMember(
                    dest => dest.Link,
                    opt => opt.MapFrom(src => src.Solution == null ? null : src.Solution.IntegrationsUrl))
                .IncludeBase<CatalogueItem, MarketingBaseModel>();

            CreateMap<CatalogueItem, MarketingBaseModel>()
                .ForMember(
                    dest => dest.BackLink,
                    opt => opt.MapFrom(src => ProfileDefaults.GetSolutionBackLink(src.CatalogueItemId)))
                .ForMember(dest => dest.BackLinkText, opt => opt.MapFrom(src => "Return to all sections"))
                .ForMember(
                    dest => dest.ClientApplication,
                    opt =>
                    {
                        opt.SetMappingOrder(0);
                        opt.MapFrom(src =>
                            src.Solution != null && !string.IsNullOrEmpty(src.Solution.ClientApplication)
                                ? JsonConvert.DeserializeObject<ClientApplication>(src.Solution.ClientApplication)
                                : new ClientApplication());
                    })
                .ForMember(dest => dest.SolutionId, opt => opt.MapFrom(src => src.CatalogueItemId))
                .ForMember(
                    dest => dest.SupplierId,
                    opt => opt.MapFrom(src => src.Supplier == null ? null : src.Supplier.Id))
                .IgnoreAllPropertiesWithAnInaccessibleSetter();

            CreateMap<CatalogueItem, RoadmapModel>()
                .ForMember(
                    dest => dest.Summary,
                    opt => opt.MapFrom(src => src.Solution == null ? null : src.Solution.RoadMap))
                .IncludeBase<CatalogueItem, MarketingBaseModel>();

            CreateMap<CatalogueItem, SolutionDescriptionModel>()
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Solution.FullDescription))
                .ForMember(dest => dest.Link, opt => opt.MapFrom(src => src.Solution.AboutUrl))
                .ForMember(dest => dest.Summary, opt => opt.MapFrom(src => src.Solution.Summary))
                .IncludeBase<CatalogueItem, MarketingBaseModel>();

            CreateMap<ContactDetailsModel, SupplierContactsModel>()
                .ForMember(dest => dest.Contacts, opt => opt.MapFrom(src => new[] { src.Contact1, src.Contact2 }))
                .ForMember(dest => dest.SolutionId, opt => opt.MapFrom(src => src.SolutionId));

            CreateMap<FeaturesModel, string[]>()
                .ConstructUsing(model => GetArrayFrom(model));
        }

        private static string[] GetArrayFrom(FeaturesModel model)
        {
            var features = new List<string>();

            if (!string.IsNullOrEmpty(model.Listing1)) features.Add(model.Listing1);
            if (!string.IsNullOrEmpty(model.Listing2)) features.Add(model.Listing2);
            if (!string.IsNullOrEmpty(model.Listing3)) features.Add(model.Listing3);
            if (!string.IsNullOrEmpty(model.Listing4)) features.Add(model.Listing4);
            if (!string.IsNullOrEmpty(model.Listing5)) features.Add(model.Listing5);
            if (!string.IsNullOrEmpty(model.Listing6)) features.Add(model.Listing6);
            if (!string.IsNullOrEmpty(model.Listing7)) features.Add(model.Listing7);
            if (!string.IsNullOrEmpty(model.Listing8)) features.Add(model.Listing8);
            if (!string.IsNullOrEmpty(model.Listing9)) features.Add(model.Listing9);
            if (!string.IsNullOrEmpty(model.Listing10)) features.Add(model.Listing10);

            return features.ToArray();
        }
    }
}
