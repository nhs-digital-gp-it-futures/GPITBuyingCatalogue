using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Constants;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.MappingProfiles
{
    public sealed class SolutionsProfile : Profile
    {
        private static readonly List<Func<CatalogueItem, bool>> ShowSectionFunctions = new()
        {
            { _ => true },
            { catalogueItem => catalogueItem.HasFeatures() },
            { catalogueItem => catalogueItem.HasCapabilities() },
            { catalogueItem => catalogueItem.HasListPrice() },
            { catalogueItem => catalogueItem.HasAdditionalServices() },
            { catalogueItem => catalogueItem.HasAssociatedServices() },
            { catalogueItem => catalogueItem.HasInteroperability() },
            { catalogueItem => catalogueItem.HasImplementationDetail() },
            { catalogueItem => catalogueItem.HasClientApplication() },
            { catalogueItem => catalogueItem.HasHosting() },
            { catalogueItem => catalogueItem.HasServiceLevelAgreement() },
            { catalogueItem => catalogueItem.HasDevelopmentPlans() },
            { catalogueItem => catalogueItem.HasSupplierDetails() },
        };

        public SolutionsProfile()
        {
            CreateMap<CatalogueItem, SolutionDisplayBaseModel>()
                .BeforeMap(
                    (src, dest) => dest.ClientApplication =
                        src.Solution != null && !string.IsNullOrEmpty(src.Solution.ClientApplication)
                            ? JsonConvert.DeserializeObject<ClientApplication>(src.Solution.ClientApplication)
                            : new ClientApplication())
                .ForMember(dest => dest.ClientApplication, opt => opt.Ignore())
                .ForMember(dest => dest.LastReviewed, opt => opt.MapFrom(src => src.Solution.LastUpdated))
                .ForMember(dest => dest.PaginationFooter, opt => opt.Ignore())
                .ForMember(dest => dest.SolutionId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.SolutionName, opt => opt.MapFrom(src => src.Name))
                .IgnoreAllPropertiesWithAnInaccessibleSetter()
                .AfterMap(
                    (src, dest) =>
                    {
                        for (int i = 0; i < ShowSectionFunctions.Count; i++)
                        {
                            if (ShowSectionFunctions[i](src))
                            {
                                dest.SetShowTrue(i);
                            }
                        }

                        dest.SetPaginationFooter();
                    });

            CreateMap<CatalogueItem, HostingTypesModel>()
                .BeforeMap(
                (src, dest) =>
                {
                    var hosting = src.Solution.GetHosting() ?? new Hosting();
                    dest.PublicCloud = hosting.PublicCloud;
                    dest.PrivateCloud = hosting.PrivateCloud;
                    dest.HybridHostingType = hosting.HybridHostingType;
                    dest.OnPremise = hosting.OnPremise;
                })
                .ForMember(dest => dest.PublicCloud, opt => opt.Ignore())
                .ForMember(dest => dest.PrivateCloud, opt => opt.Ignore())
                .ForMember(dest => dest.HybridHostingType, opt => opt.Ignore())
                .ForMember(dest => dest.OnPremise, opt => opt.Ignore())
                .IncludeBase<CatalogueItem, SolutionDisplayBaseModel>();

            CreateMap<CatalogueItem, ListPriceModel>()
                .ForMember(
                    dest => dest.FlatListPrices,
                    opt =>
                    {
                        opt.PreCondition(src => src.CataloguePrices != null);
                        opt.MapFrom(src => src.CataloguePrices.Where(p => p.CataloguePriceType == CataloguePriceType.Flat));
                    })
                .IncludeBase<CatalogueItem, SolutionDisplayBaseModel>();

            CreateMap<CataloguePrice, PriceViewModel>()
                .ForMember(
                    dest => dest.CurrencyCode,
                    opt => opt.MapFrom(
                        src => CurrencyCodeSigns.Code.ContainsKey(src.CurrencyCode)
                            ? CurrencyCodeSigns.Code[src.CurrencyCode]
                            : null))
                .ForMember(
                    dest => dest.Price,
                    opt =>
                    {
                        opt.PreCondition(src => src.Price != null);
                        opt.MapFrom(src => Math.Round(src.Price.Value, 2));
                    })
                .ForMember(
                    dest => dest.Unit,
                    opt => opt.MapFrom(
                        src =>
                            $"{(src.PricingUnit == null ? string.Empty : src.PricingUnit.Description)} {(src.TimeUnit == null ? string.Empty : src.TimeUnit.Value.Description())}"));
        }
    }
}
