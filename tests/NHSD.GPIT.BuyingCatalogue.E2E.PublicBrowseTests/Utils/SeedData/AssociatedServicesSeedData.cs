using System;
using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.SeedData
{
    public static class AssociatedServicesSeedData
    {
        public static List<CatalogueItem> GetAssociatedServices()
        {
            return new List<CatalogueItem>
            {
                new()
                {
                    Id = new CatalogueItemId(99999, "S-999"),
                    Name = "Associated Service For Test",
                    CatalogueItemType = CatalogueItemType.AssociatedService,
                    Created = DateTime.UtcNow,
                    PublishedStatus = PublicationStatus.Published,
                    SupplierId = 99999,
                    AssociatedService =
                        new AssociatedService
                        {
                            Description = "This is the description of the Associated Service",
                            OrderGuidance = "This is how to order",
                            LastUpdated = DateTime.UtcNow,
                        },
                    CataloguePrices = new List<CataloguePrice>()
                    {
                        new()
                        {
                            CataloguePriceId = 23,
                            CatalogueItemId = new CatalogueItemId(99999, "S-999"),
                            ProvisioningType = ProvisioningType.Patient,
                            CataloguePriceType = CataloguePriceType.Tiered,
                            CataloguePriceCalculationType = CataloguePriceCalculationType.Cumulative,
                            PublishedStatus = PublicationStatus.Published,
                            PricingUnit =
                                new()
                                {
                                    Id = 30,
                                    RangeDescription = "Test Tier",
                                    Description = "per patient",
                                },
                            TimeUnit = TimeUnit.PerYear,
                            CurrencyCode = "GBP",
                            LastUpdated = DateTime.UtcNow,
                            CataloguePriceTiers = new List<CataloguePriceTier>
                            {
                                new()
                                {
                                    Id = 30,
                                    CataloguePriceId = 23,
                                    LowerRange = 1,
                                    UpperRange = 99,
                                    Price = 100.9999M,
                                    LastUpdated = DateTime.UtcNow,
                                },
                                new()
                                {
                                    Id = 31,
                                    CataloguePriceId = 23,
                                    LowerRange = 100,
                                    UpperRange = 299,
                                    Price = 50.9999M,
                                    LastUpdated = DateTime.UtcNow,
                                },
                                new()
                                {
                                    Id = 32,
                                    CataloguePriceId = 23,
                                    LowerRange = 300,
                                    UpperRange = null,
                                    Price = 25.9999M,
                                    LastUpdated = DateTime.UtcNow,
                                },
                            },
                        },
                    },
                },
                new()
                {
                    Id = new CatalogueItemId(99999, "S-998"),
                    Name = "Duplicate Associated Service For Test",
                    CatalogueItemType = CatalogueItemType.AssociatedService,
                    Created = DateTime.UtcNow,
                    PublishedStatus = PublicationStatus.Published,
                    SupplierId = 99999,
                    AssociatedService = new AssociatedService
                    {
                        Description = "This is the description of the Associated Service",
                        OrderGuidance = "This is how to order",
                        LastUpdated = DateTime.UtcNow,
                    },
                    CataloguePrices = new List<CataloguePrice>()
                    {
                        new()
                        {
                            CataloguePriceId = 24,
                            CatalogueItemId = new CatalogueItemId(99999, "S-998"),
                            ProvisioningType = ProvisioningType.Patient,
                            CataloguePriceType = CataloguePriceType.Flat,
                            PublishedStatus = PublicationStatus.Published,
                            CataloguePriceCalculationType = CataloguePriceCalculationType.SingleFixed,
                            PricingUnit =
                                new PricingUnit
                                {
                                    Id = 31,
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
                                    Id = 33,
                                    CataloguePriceId = 24,
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
                    Id = new CatalogueItemId(99998, "S-997"),
                    Name = "E2E Multiple Prices Associated Service",
                    CatalogueItemType = CatalogueItemType.AssociatedService,
                    Created = DateTime.UtcNow,
                    PublishedStatus = PublicationStatus.Published,
                    SupplierId = 99998,
                    AssociatedService =
                        new AssociatedService
                        {
                            Description = "This is the description of the Associated Service",
                            OrderGuidance = "This is how to order",
                            LastUpdated = DateTime.UtcNow,
                        },
                    CataloguePrices = new List<CataloguePrice>
                    {
                        new()
                        {
                            CataloguePriceId = 16,
                            CatalogueItemId = new CatalogueItemId(99998, "S-997"),
                            ProvisioningType = ProvisioningType.Patient,
                            CataloguePriceType = CataloguePriceType.Flat,
                            PublishedStatus = PublicationStatus.Published,
                            CataloguePriceCalculationType = CataloguePriceCalculationType.SingleFixed,
                            PricingUnit =
                                new PricingUnit
                                {
                                    Id = 16, RangeDescription = "Test Tier", Description = "per test patient",
                                },
                            TimeUnit = TimeUnit.PerYear,
                            CurrencyCode = "GBP",
                            LastUpdated = DateTime.UtcNow,
                            CataloguePriceTiers = new List<CataloguePriceTier>
                            {
                                new()
                                {
                                    Id = 21,
                                    CataloguePriceId = 16,
                                    LowerRange = 1,
                                    UpperRange = null,
                                    Price = 999.9999M,
                                    LastUpdated = DateTime.UtcNow,
                                },
                            },
                        },
                        new()
                        {
                            CataloguePriceId = 17,
                            CatalogueItemId = new CatalogueItemId(99998, "S-997"),
                            ProvisioningType = ProvisioningType.OnDemand,
                            CataloguePriceType = CataloguePriceType.Flat,
                            PublishedStatus = PublicationStatus.Published,
                            CataloguePriceCalculationType = CataloguePriceCalculationType.SingleFixed,
                            PricingUnit =
                                new PricingUnit
                                {
                                    Id = 17, RangeDescription = "Test Tier", Description = "per test on demand",
                                },
                            TimeUnit = TimeUnit.PerYear,
                            CurrencyCode = "GBP",
                            LastUpdated = DateTime.UtcNow,
                            CataloguePriceTiers = new List<CataloguePriceTier>
                            {
                                new()
                                {
                                    Id = 22,
                                    CataloguePriceId = 17,
                                    LowerRange = 1,
                                    UpperRange = null,
                                    Price = 999.9999M,
                                    LastUpdated = DateTime.UtcNow,
                                },
                            },
                        },
                        new()
                        {
                            CataloguePriceId = 18,
                            CatalogueItemId = new CatalogueItemId(99998, "S-997"),
                            ProvisioningType = ProvisioningType.Declarative,
                            CataloguePriceType = CataloguePriceType.Flat,
                            PublishedStatus = PublicationStatus.Published,
                            CataloguePriceCalculationType = CataloguePriceCalculationType.SingleFixed,
                            PricingUnit =
                                new PricingUnit
                                {
                                    Id = 18, RangeDescription = "Test Tier", Description = "per test declarative",
                                },
                            TimeUnit = TimeUnit.PerYear,
                            CurrencyCode = "GBP",
                            LastUpdated = DateTime.UtcNow,
                            CataloguePriceTiers = new List<CataloguePriceTier>
                            {
                                new()
                                {
                                    Id = 23,
                                    CataloguePriceId = 18,
                                    LowerRange = 1,
                                    UpperRange = null,
                                    Price = 999.9999M,
                                    LastUpdated = DateTime.UtcNow,
                                },
                            },
                        },
                        new()
                        {
                            CataloguePriceId = 19,
                            CatalogueItemId = new CatalogueItemId(99998, "S-997"),
                            ProvisioningType = ProvisioningType.Patient,
                            CataloguePriceType = CataloguePriceType.Flat,
                            PublishedStatus = PublicationStatus.Draft,
                            CataloguePriceCalculationType = CataloguePriceCalculationType.SingleFixed,
                            PricingUnit =
                                new PricingUnit
                                {
                                    Id = 19, RangeDescription = "Test Tier", Description = "per test declarative",
                                },
                            TimeUnit = TimeUnit.PerYear,
                            CurrencyCode = "GBP",
                            LastUpdated = DateTime.UtcNow,
                            CataloguePriceTiers = new List<CataloguePriceTier>
                            {
                                new()
                                {
                                    Id = 24,
                                    CataloguePriceId = 19,
                                    LowerRange = 1,
                                    UpperRange = null,
                                    Price = 999.9999M,
                                    LastUpdated = DateTime.UtcNow,
                                },
                            },
                        },
                        new()
                        {
                            CataloguePriceId = 20,
                            CatalogueItemId = new CatalogueItemId(99998, "S-997"),
                            ProvisioningType = ProvisioningType.Patient,
                            CataloguePriceType = CataloguePriceType.Tiered,
                            PublishedStatus = PublicationStatus.Draft,
                            CataloguePriceCalculationType = CataloguePriceCalculationType.Volume,
                            PricingUnit =
                                new PricingUnit
                                {
                                    Id = 20,
                                    RangeDescription = "Test Tier",
                                    Description = "per tiered price",
                                },
                            TimeUnit = TimeUnit.PerYear,
                            CurrencyCode = "GBP",
                            LastUpdated = DateTime.UtcNow,
                            CataloguePriceTiers = new List<CataloguePriceTier>
                            {
                                new()
                                {
                                    Id = 25,
                                    CataloguePriceId = 20,
                                    LowerRange = 1,
                                    UpperRange = 99,
                                    Price = 100.9999M,
                                    LastUpdated = DateTime.UtcNow,
                                },
                                new()
                                {
                                    Id = 26,
                                    CataloguePriceId = 20,
                                    LowerRange = 100,
                                    UpperRange = 299,
                                    Price = 50.9999M,
                                    LastUpdated = DateTime.UtcNow,
                                },
                                new()
                                {
                                    Id = 27,
                                    CataloguePriceId = 20,
                                    LowerRange = 300,
                                    UpperRange = null,
                                    Price = 25.9999M,
                                    LastUpdated = DateTime.UtcNow,
                                },
                            },
                        },
                        new()
                        {
                            CataloguePriceId = 29,
                            CatalogueItemId = new CatalogueItemId(99998, "S-997"),
                            ProvisioningType = ProvisioningType.Patient,
                            CataloguePriceType = CataloguePriceType.Tiered,
                            PublishedStatus = PublicationStatus.Draft,
                            CataloguePriceCalculationType = CataloguePriceCalculationType.Volume,
                            PricingUnit =
                                new PricingUnit
                                {
                                    Id = 999,
                                    RangeDescription = "Test Tier",
                                    Description = "maximum tiers",
                                },
                            TimeUnit = TimeUnit.PerYear,
                            CurrencyCode = "GBP",
                            LastUpdated = DateTime.UtcNow,
                            CataloguePriceTiers = new List<CataloguePriceTier>
                            {
                                new()
                                {
                                    Id = 52,
                                    CataloguePriceId = 29,
                                    LowerRange = 1,
                                    UpperRange = 9999,
                                    Price = 999.9999M,
                                    LastUpdated = DateTime.UtcNow,
                                },
                                new()
                                {
                                    Id = 53,
                                    CataloguePriceId = 29,
                                    LowerRange = 10000,
                                    UpperRange = 49000,
                                    LastUpdated = DateTime.UtcNow,
                                },
                                new()
                                {
                                    Id = 54,
                                    CataloguePriceId = 29,
                                    LowerRange = 50000,
                                    UpperRange = 99999,
                                    LastUpdated = DateTime.UtcNow,
                                },
                                new()
                                {
                                    Id = 55,
                                    CataloguePriceId = 29,
                                    LowerRange = 100000,
                                    UpperRange = 149999,
                                    LastUpdated = DateTime.UtcNow,
                                },
                                new()
                                {
                                    Id = 56,
                                    CataloguePriceId = 29,
                                    LowerRange = 150000,
                                    UpperRange = null,
                                    LastUpdated = DateTime.UtcNow,
                                },
                                new()
                                {
                                    Id = 57,
                                    CataloguePriceId = 29,
                                    LowerRange = 150000,
                                    UpperRange = null,
                                    LastUpdated = DateTime.UtcNow,
                                },
                                new()
                                {
                                    Id = 58,
                                    CataloguePriceId = 29,
                                    LowerRange = 150000,
                                    UpperRange = null,
                                    LastUpdated = DateTime.UtcNow,
                                },
                            },
                        },
                    },
                },
                new()
                {
                    Id = new CatalogueItemId(99998, "S-998"),
                    Name = "E2E Single Price Associated Service",
                    CatalogueItemType = CatalogueItemType.AssociatedService,
                    Created = DateTime.UtcNow,
                    PublishedStatus = PublicationStatus.Published,
                    SupplierId = 99998,
                    AssociatedService =
                        new AssociatedService
                        {
                            Description = "This is the description of the Associated Service",
                            OrderGuidance = "This is how to order",
                            LastUpdated = DateTime.UtcNow,
                        },
                    CataloguePrices = new List<CataloguePrice>
                    {
                        new()
                        {
                            CataloguePriceId = 21,
                            CatalogueItemId = new CatalogueItemId(99998, "S-998"),
                            ProvisioningType = ProvisioningType.Patient,
                            CataloguePriceType = CataloguePriceType.Flat,
                            PublishedStatus = PublicationStatus.Published,
                            CataloguePriceCalculationType = CataloguePriceCalculationType.SingleFixed,
                            PricingUnit =
                                new PricingUnit
                                {
                                    Id = 21, RangeDescription = "Test Tier", Description = "per test patient",
                                },
                            TimeUnit = TimeUnit.PerYear,
                            CurrencyCode = "GBP",
                            LastUpdated = DateTime.UtcNow,
                            CataloguePriceTiers = new List<CataloguePriceTier>
                            {
                                new()
                                {
                                    Id = 28,
                                    CataloguePriceId = 21,
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
                    Id = new CatalogueItemId(99998, "S-999"),
                    Name = "E2E Single Price Added Associated Service",
                    CatalogueItemType = CatalogueItemType.AssociatedService,
                    Created = DateTime.UtcNow,
                    PublishedStatus = PublicationStatus.Published,
                    SupplierId = 99998,
                    AssociatedService =
                        new AssociatedService
                        {
                            Description = "This is the description of the Associated Service",
                            OrderGuidance = "This is how to order",
                            LastUpdated = DateTime.UtcNow,
                        },
                    CataloguePrices = new List<CataloguePrice>
                    {
                        new()
                        {
                            CataloguePriceId = 22,
                            CatalogueItemId = new CatalogueItemId(99998, "S-999"),
                            ProvisioningType = ProvisioningType.Patient,
                            CataloguePriceType = CataloguePriceType.Flat,
                            PublishedStatus = PublicationStatus.Published,
                            CataloguePriceCalculationType = CataloguePriceCalculationType.SingleFixed,
                            PricingUnit =
                                new PricingUnit
                                {
                                    Id = 22, RangeDescription = "Test Tier", Description = "per test patient",
                                },
                            TimeUnit = TimeUnit.PerYear,
                            CurrencyCode = "GBP",
                            LastUpdated = DateTime.UtcNow,
                            CataloguePriceTiers = new List<CataloguePriceTier>
                            {
                                new()
                                {
                                    Id = 29,
                                    CataloguePriceId = 22,
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
