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
                    AssociatedService = new AssociatedService
                    {
                        Description = "This is the description of the Associated Service",
                        OrderGuidance = "This is how to order",
                        LastUpdated = DateTime.UtcNow,
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
                },
                new()
                {
                    Id = new CatalogueItemId(99998, "S-997"),
                    Name = "E2E Multiple Prices Associated Service",
                    CatalogueItemType = CatalogueItemType.AssociatedService,
                    Created = DateTime.UtcNow,
                    PublishedStatus = PublicationStatus.Published,
                    SupplierId = 99998,
                    AssociatedService = new AssociatedService
                    {
                        Description = "This is the description of the Associated Service",
                        OrderGuidance = "This is how to order",
                        LastUpdated = DateTime.UtcNow,
                    },
                    CataloguePrices = new List<CataloguePrice>
                    {
                        new()
                        {
                            CataloguePriceId = 10,
                            CatalogueItemId = new CatalogueItemId(99998, "S-997"),
                            ProvisioningType = ProvisioningType.Patient,
                            CataloguePriceType = CataloguePriceType.Flat,
                            PricingUnit = new PricingUnit
                            {
                                Id = 10,
                                TierName = "Test Tier",
                                Description = "per test patient",
                            },
                            TimeUnit = TimeUnit.PerYear,
                            CurrencyCode = "GBP",
                            Price = 999.9999M,
                            LastUpdated = DateTime.UtcNow,
                        },
                        new()
                        {
                            CataloguePriceId = 11,
                            CatalogueItemId = new CatalogueItemId(99998, "S-997"),
                            ProvisioningType = ProvisioningType.OnDemand,
                            CataloguePriceType = CataloguePriceType.Flat,
                            PricingUnit = new PricingUnit
                            {
                                Id = 11,
                                TierName = "Test Tier",
                                Description = "per test on demand",
                            },
                            TimeUnit = TimeUnit.PerYear,
                            CurrencyCode = "GBP",
                            Price = 999.9999M,
                            LastUpdated = DateTime.UtcNow,
                        },
                        new()
                        {
                            CataloguePriceId = 12,
                            CatalogueItemId = new CatalogueItemId(99998, "S-997"),
                            ProvisioningType = ProvisioningType.Declarative,
                            CataloguePriceType = CataloguePriceType.Flat,
                            PricingUnit = new PricingUnit
                            {
                                Id = 12,
                                TierName = "Test Tier",
                                Description = "per test declarative",
                            },
                            TimeUnit = TimeUnit.PerYear,
                            CurrencyCode = "GBP",
                            Price = 999.9999M,
                            LastUpdated = DateTime.UtcNow,
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
                    AssociatedService = new AssociatedService
                    {
                        Description = "This is the description of the Associated Service",
                        OrderGuidance = "This is how to order",
                        LastUpdated = DateTime.UtcNow,
                    },
                    CataloguePrices = new List<CataloguePrice>
                    {
                        new()
                        {
                            CataloguePriceId = 13,
                            CatalogueItemId = new CatalogueItemId(99998, "S-998"),
                            ProvisioningType = ProvisioningType.Patient,
                            CataloguePriceType = CataloguePriceType.Flat,
                            PricingUnit = new PricingUnit
                            {
                                Id = 13,
                                TierName = "Test Tier",
                                Description = "per test patient",
                            },
                            TimeUnit = TimeUnit.PerYear,
                            CurrencyCode = "GBP",
                            Price = 999.9999M,
                            LastUpdated = DateTime.UtcNow,
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
                    AssociatedService = new AssociatedService
                    {
                        Description = "This is the description of the Associated Service",
                        OrderGuidance = "This is how to order",
                        LastUpdated = DateTime.UtcNow,
                    },
                    CataloguePrices = new List<CataloguePrice>
                    {
                        new()
                        {
                            CataloguePriceId = 14,
                            CatalogueItemId = new CatalogueItemId(99998, "S-999"),
                            ProvisioningType = ProvisioningType.Patient,
                            CataloguePriceType = CataloguePriceType.Flat,
                            PricingUnit = new PricingUnit
                            {
                                Id = 14,
                                TierName = "Test Tier",
                                Description = "per test patient",
                            },
                            TimeUnit = TimeUnit.PerYear,
                            CurrencyCode = "GBP",
                            Price = 999.9999M,
                            LastUpdated = DateTime.UtcNow,
                        },
                    },
                },
            };
        }
    }
}
