using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.SeedData
{
    public static class AdditionalServicesSeedData
    {
        public static List<CatalogueItem> GetAdditionalServices(IList<CatalogueItem> catalogueSolutions)
        {
            return new List<CatalogueItem>
            {
                new()
                {
                    Id = new CatalogueItemId(99999, "001A99"),
                    Name = "Additional service",
                    CatalogueItemType = CatalogueItemType.AdditionalService,
                    Created = DateTime.UtcNow,
                    PublishedStatus = PublicationStatus.Published,
                    SupplierId = 99999,
                    AdditionalService = new AdditionalService
                    {
                        Summary = "This is the summary of the Additional Service",
                        FullDescription = "This is the description of the Additional Service",
                        LastUpdated = DateTime.UtcNow,
                        Solution = catalogueSolutions.Single(s => s.Id == new CatalogueItemId(99999, "001")).Solution,
                    },
                    CatalogueItemCapabilities = new List<CatalogueItemCapability>
                    {
                        new()
                        {
                            CatalogueItemId = new CatalogueItemId(99999, "001A99"),
                            CapabilityId = 2,
                            LastUpdated = DateTime.UtcNow,
                            StatusId = 1,
                        },
                    },
                    CatalogueItemEpics = new List<CatalogueItemEpic>
                    {
                        new()
                        {
                            CatalogueItemId = new CatalogueItemId(99999, "001A99"),
                            CapabilityId = 2,
                            EpicId = "C2E1",
                            StatusId = 1,
                        },
                        new()
                        {
                            CatalogueItemId = new CatalogueItemId(99999, "001A99"),
                            CapabilityId = 2,
                            EpicId = "C2E2",
                            StatusId = 1,
                        },
                        new()
                        {
                            CatalogueItemId = new CatalogueItemId(99999, "001A99"),
                            CapabilityId = 2,
                            EpicId = "E123456",
                            StatusId = 1,
                        },
                    },
                    CataloguePrices = new List<CataloguePrice>()
                    {
                        new()
                        {
                            CataloguePriceId = 25,
                            CatalogueItemId = new CatalogueItemId(99999, "001A99"),
                            ProvisioningType = ProvisioningType.Patient,
                            CataloguePriceType = CataloguePriceType.Tiered,
                            CataloguePriceCalculationType = CataloguePriceCalculationType.Cumulative,
                            PublishedStatus = PublicationStatus.Published,
                            PricingUnit = new PricingUnit
                            {
                                Id = 32,
                                RangeDescription = "Test Tier",
                                Description = "per tiered test declarative",
                            },
                            TimeUnit = TimeUnit.PerYear,
                            CurrencyCode = "GBP",
                            LastUpdated = DateTime.UtcNow,
                            CataloguePriceTiers = new List<CataloguePriceTier>
                            {
                                new()
                                {
                                    Id = 34,
                                    CataloguePriceId = 25,
                                    LowerRange = 1,
                                    UpperRange = 89999,
                                    Price = 999.9999M,
                                    LastUpdated = DateTime.UtcNow,
                                },
                                new()
                                {
                                    Id = 35,
                                    CataloguePriceId = 25,
                                    LowerRange = 90000,
                                    UpperRange = 499999,
                                    Price = 999.9999M,
                                    LastUpdated = DateTime.UtcNow,
                                },
                                new()
                                {
                                    Id = 36,
                                    CataloguePriceId = 25,
                                    LowerRange = 500000,
                                    UpperRange = null,
                                    Price = 999.9999M,
                                    LastUpdated = DateTime.UtcNow,
                                },
                            },
                        },
                    },
                },
                new()
                {
                    Id = new CatalogueItemId(99999, "001A98"),
                    Name = "Additional service 2",
                    CatalogueItemType = CatalogueItemType.AdditionalService,
                    Created = DateTime.UtcNow,
                    PublishedStatus = PublicationStatus.Published,
                    SupplierId = 99999,
                    AdditionalService = new AdditionalService
                    {
                        Summary = "This is the summary of the Additional Service 2",
                        FullDescription = "This is the description of the Additional Service 2",
                        LastUpdated = DateTime.UtcNow,
                        Solution = catalogueSolutions.Single(s => s.Id == new CatalogueItemId(99999, "001")).Solution,
                    },
                    CatalogueItemCapabilities = new List<CatalogueItemCapability>
                    {
                        new()
                        {
                            CatalogueItemId = new CatalogueItemId(99999, "001A98"),
                            CapabilityId = 2,
                            LastUpdated = DateTime.UtcNow,
                            StatusId = 1,
                        },
                    },
                    CatalogueItemEpics = new List<CatalogueItemEpic>
                    {
                        new()
                        {
                            CatalogueItemId = new CatalogueItemId(99999, "001A98"),
                            CapabilityId = 2,
                            EpicId = "C2E1",
                            StatusId = 1,
                        },
                        new()
                        {
                            CatalogueItemId = new CatalogueItemId(99999, "001A98"),
                            CapabilityId = 2,
                            EpicId = "C2E2",
                            StatusId = 1,
                        },
                        new()
                        {
                            CatalogueItemId = new CatalogueItemId(99999, "001A98"),
                            CapabilityId = 2,
                            EpicId = "E123456",
                            StatusId = 1,
                        },
                        new()
                        {
                            CatalogueItemId = new CatalogueItemId(99999, "001A98"),
                            CapabilityId = 2,
                            EpicId = "S00004",
                            StatusId = 1,
                        },
                    },
                    CataloguePrices = new List<CataloguePrice>()
                    {
                        new()
                        {
                            CataloguePriceId = 26,
                            CatalogueItemId = new CatalogueItemId(99999, "001A98"),
                            ProvisioningType = ProvisioningType.Patient,
                            CataloguePriceType = CataloguePriceType.Flat,
                            CataloguePriceCalculationType = CataloguePriceCalculationType.SingleFixed,
                            PublishedStatus = PublicationStatus.Published,
                            PricingUnit = new PricingUnit
                            {
                                Id = 33,
                                RangeDescription = "Test Tier",
                                Description = "per test patient",
                            },
                            TimeUnit = TimeUnit.PerYear,
                            CurrencyCode = "GBP",
                            LastUpdated = DateTime.UtcNow,
                            CataloguePriceTiers = new List<CataloguePriceTier>
                            {
                                new()
                                {
                                    Id = 37,
                                    CataloguePriceId = 10,
                                    LowerRange = 1,
                                    UpperRange = null,
                                    Price = 999.9999M,
                                    LastUpdated = DateTime.UtcNow,
                                },
                            },
                        },
                    },
                },
                new()
                {
                    Id = new CatalogueItemId(99998, "001A99"),
                    Name = "E2E Multiple Prices Additional Service",
                    CatalogueItemType = CatalogueItemType.AdditionalService,
                    Created = DateTime.UtcNow,
                    PublishedStatus = PublicationStatus.Published,
                    SupplierId = 99998,
                    AdditionalService = new AdditionalService
                    {
                        Summary = "This is the summary of the Additional Service",
                        FullDescription = "This is the description of the Additional Service",
                        LastUpdated = DateTime.UtcNow,
                        Solution = catalogueSolutions.Single(s => s.Id == new CatalogueItemId(99998, "001")).Solution,
                    },
                    CatalogueItemCapabilities = new List<CatalogueItemCapability>
                    {
                        new()
                        {
                            CatalogueItemId = new CatalogueItemId(99998, "001A99"),
                            CapabilityId = 2,
                            LastUpdated = DateTime.UtcNow,
                            StatusId = 1,
                        },
                    },
                    CatalogueItemEpics = new List<CatalogueItemEpic>
                    {
                        new()
                        {
                            CatalogueItemId = new CatalogueItemId(99998, "001A99"),
                            CapabilityId = 27,
                            EpicId = "C27E4",
                            StatusId = 1,
                        },
                        new()
                        {
                            CatalogueItemId = new CatalogueItemId(99998, "001A99"),
                            CapabilityId = 27,
                            EpicId = "C27E5",
                            StatusId = 1,
                        },
                        new()
                        {
                            CatalogueItemId = new CatalogueItemId(99998, "001A99"),
                            CapabilityId = 27,
                            EpicId = "C27E6",
                            StatusId = 1,
                        },
                    },
                    CataloguePrices = new List<CataloguePrice>
                    {
                        new()
                        {
                            CataloguePriceId = 10,
                            CatalogueItemId = new CatalogueItemId(99998, "001A99"),
                            ProvisioningType = ProvisioningType.Patient,
                            CataloguePriceType = CataloguePriceType.Flat,
                            CataloguePriceCalculationType = CataloguePriceCalculationType.SingleFixed,
                            PublishedStatus = PublicationStatus.Published,
                            PricingUnit = new PricingUnit
                            {
                                Id = 10,
                                RangeDescription = "Test Tier",
                                Description = "per test patient",
                            },
                            TimeUnit = TimeUnit.PerYear,
                            CurrencyCode = "GBP",
                            LastUpdated = DateTime.UtcNow,
                            CataloguePriceTiers = new List<CataloguePriceTier>
                            {
                                new()
                                {
                                    Id = 13,
                                    CataloguePriceId = 10,
                                    LowerRange = 1,
                                    UpperRange = null,
                                    Price = 999.9999M,
                                    LastUpdated = DateTime.UtcNow,
                                },
                            },
                        },
                        new()
                        {
                            CataloguePriceId = 11,
                            CatalogueItemId = new CatalogueItemId(99998, "001A99"),
                            ProvisioningType = ProvisioningType.OnDemand,
                            CataloguePriceType = CataloguePriceType.Flat,
                            PublishedStatus = PublicationStatus.Published,
                            CataloguePriceCalculationType = CataloguePriceCalculationType.SingleFixed,
                            PricingUnit = new PricingUnit
                            {
                                Id = 11,
                                RangeDescription = "Test Tier",
                                Description = "per test on demand",
                            },
                            TimeUnit = TimeUnit.PerYear,
                            CurrencyCode = "GBP",
                            LastUpdated = DateTime.UtcNow,
                            CataloguePriceTiers = new List<CataloguePriceTier>
                            {
                                new()
                                {
                                    Id = 14,
                                    CataloguePriceId = 11,
                                    LowerRange = 1,
                                    UpperRange = null,
                                    Price = 999.9999M,
                                    LastUpdated = DateTime.UtcNow,
                                },
                            },
                        },
                        new()
                        {
                            CataloguePriceId = 12,
                            CatalogueItemId = new CatalogueItemId(99998, "001A99"),
                            ProvisioningType = ProvisioningType.Declarative,
                            CataloguePriceType = CataloguePriceType.Flat,
                            CataloguePriceCalculationType = CataloguePriceCalculationType.SingleFixed,
                            PublishedStatus = PublicationStatus.Published,
                            PricingUnit = new PricingUnit
                            {
                                Id = 12,
                                RangeDescription = "Test Tier",
                                Description = "per test declarative",
                            },
                            TimeUnit = TimeUnit.PerYear,
                            CurrencyCode = "GBP",
                            LastUpdated = DateTime.UtcNow,
                            CataloguePriceTiers = new List<CataloguePriceTier>
                            {
                                new()
                                {
                                    Id = 15,
                                    CataloguePriceId = 12,
                                    LowerRange = 1,
                                    UpperRange = null,
                                    Price = 999.9999M,
                                    LastUpdated = DateTime.UtcNow,
                                },
                            },
                        },
                        new()
                        {
                            CataloguePriceId = 13,
                            CatalogueItemId = new CatalogueItemId(99998, "001A99"),
                            ProvisioningType = ProvisioningType.Patient,
                            CataloguePriceType = CataloguePriceType.Flat,
                            CataloguePriceCalculationType = CataloguePriceCalculationType.SingleFixed,
                            PublishedStatus = PublicationStatus.Draft,
                            PricingUnit = new PricingUnit
                            {
                                Id = 13,
                                RangeDescription = "Test Tier",
                                Description = "per test declarative",
                            },
                            TimeUnit = TimeUnit.PerYear,
                            CurrencyCode = "GBP",
                            LastUpdated = DateTime.UtcNow,
                            CataloguePriceTiers = new List<CataloguePriceTier>
                            {
                                new()
                                {
                                    Id = 16,
                                    CataloguePriceId = 13,
                                    LowerRange = 1,
                                    UpperRange = null,
                                    Price = 999.9999M,
                                    LastUpdated = DateTime.UtcNow,
                                },
                            },
                        },
                        new()
                        {
                            CataloguePriceId = 14,
                            CatalogueItemId = new CatalogueItemId(99998, "001A99"),
                            ProvisioningType = ProvisioningType.Patient,
                            CataloguePriceType = CataloguePriceType.Tiered,
                            CataloguePriceCalculationType = CataloguePriceCalculationType.Cumulative,
                            PublishedStatus = PublicationStatus.Draft,
                            PricingUnit = new PricingUnit
                            {
                                Id = 14,
                                RangeDescription = "Test Tier",
                                Description = "per tiered test declarative",
                            },
                            TimeUnit = TimeUnit.PerYear,
                            CurrencyCode = "GBP",
                            LastUpdated = DateTime.UtcNow,
                            CataloguePriceTiers = new List<CataloguePriceTier>
                            {
                                new()
                                {
                                    Id = 17,
                                    CataloguePriceId = 14,
                                    LowerRange = 1,
                                    UpperRange = 89999,
                                    Price = 999.9999M,
                                    LastUpdated = DateTime.UtcNow,
                                },
                                new()
                                {
                                    Id = 18,
                                    CataloguePriceId = 14,
                                    LowerRange = 90000,
                                    UpperRange = 499999,
                                    Price = 999.9999M,
                                    LastUpdated = DateTime.UtcNow,
                                },
                                new()
                                {
                                    Id = 19,
                                    CataloguePriceId = 14,
                                    LowerRange = 500000,
                                    UpperRange = null,
                                    Price = 999.9999M,
                                    LastUpdated = DateTime.UtcNow,
                                },
                            },
                        },
                    },
                },
                new()
                {
                    Id = new CatalogueItemId(99998, "002A999"),
                    Name = "E2E No Contact Single Price Additional Service",
                    CatalogueItemType = CatalogueItemType.AdditionalService,
                    Created = DateTime.UtcNow,
                    PublishedStatus = PublicationStatus.Published,
                    SupplierId = 99998,
                    AdditionalService = new AdditionalService
                    {
                        Summary = "This is the summary of the Additional Service",
                        FullDescription = "This is the description of the Additional Service",
                        LastUpdated = DateTime.UtcNow,
                        Solution = catalogueSolutions.Single(s => s.Id == new CatalogueItemId(99998, "002")).Solution,
                    },
                    CatalogueItemCapabilities = new List<CatalogueItemCapability>
                    {
                        new()
                        {
                            CatalogueItemId = new CatalogueItemId(99998, "002A999"),
                            CapabilityId = 2,
                            LastUpdated = DateTime.UtcNow,
                            StatusId = 1,
                        },
                    },
                    CatalogueItemEpics = new List<CatalogueItemEpic>
                    {
                        new()
                        {
                            CatalogueItemId = new CatalogueItemId(99998, "002A999"),
                            CapabilityId = 27,
                            EpicId = "C27E4",
                            StatusId = 1,
                        },
                        new()
                        {
                            CatalogueItemId = new CatalogueItemId(99998, "002A999"),
                            CapabilityId = 27,
                            EpicId = "C27E5",
                            StatusId = 1,
                        },
                        new()
                        {
                            CatalogueItemId = new CatalogueItemId(99998, "002A999"),
                            CapabilityId = 27,
                            EpicId = "C27E6",
                            StatusId = 1,
                        },
                    },
                    CataloguePrices = new List<CataloguePrice>
                    {
                        new()
                        {
                            CataloguePriceId = 15,
                            CatalogueItemId = new CatalogueItemId(99998, "002A999"),
                            ProvisioningType = ProvisioningType.Patient,
                            CataloguePriceType = CataloguePriceType.Flat,
                            CataloguePriceCalculationType = CataloguePriceCalculationType.SingleFixed,
                            PublishedStatus = PublicationStatus.Published,
                            PricingUnit = new PricingUnit
                            {
                                Id = 15,
                                RangeDescription = "Test Tier",
                                Description = "per test declarative",
                            },
                            TimeUnit = TimeUnit.PerYear,
                            CurrencyCode = "GBP",
                            LastUpdated = DateTime.UtcNow,
                            CataloguePriceTiers = new List<CataloguePriceTier>
                            {
                                new()
                                {
                                    Id = 20,
                                    CataloguePriceId = 15,
                                    LowerRange = 1,
                                    UpperRange = null,
                                    Price = 999.9999M,
                                    LastUpdated = DateTime.UtcNow,
                                },
                            },
                        },
                    },
                },
            };
        }
    }
}
