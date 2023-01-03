using System;
using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.SeedData;

internal static class CataloguePricesSeedData
{
    internal static List<CataloguePrice> GetCataloguePrices()
    {
        return new List<CataloguePrice>
        {
            new()
            {
                CataloguePriceId = -2,
                CatalogueItemId = new CatalogueItemId(99999, "S-999"),
                CataloguePriceType = CataloguePriceType.Flat,
                PricingUnit = new PricingUnit { RangeDescription = "things", Description = "per thing", },
                CurrencyCode = "GBP",
                ProvisioningType = ProvisioningType.Declarative,
                TimeUnit = TimeUnit.PerYear,
                LastUpdated = DateTime.UtcNow,
            },
            new CataloguePrice
            {
                CataloguePriceId = -1,
                CatalogueItemId = new CatalogueItemId(99999, "001"),
                CataloguePriceType = CataloguePriceType.Flat,
                PricingUnit = new PricingUnit { RangeDescription = "patients", Description = "per patient", },
                CurrencyCode = "GBP",
                ProvisioningType = ProvisioningType.Patient,
                TimeUnit = TimeUnit.PerYear,
                LastUpdated = DateTime.UtcNow,
            },
            new()
            {
                CataloguePriceId = 1,
                CatalogueItemId = new CatalogueItemId(99998, "001"),
                ProvisioningType = ProvisioningType.Patient,
                CataloguePriceCalculationType = CataloguePriceCalculationType.Volume,
                CataloguePriceType = CataloguePriceType.Flat,
                PublishedStatus = PublicationStatus.Published,
                PricingUnit = new PricingUnit { RangeDescription = "Patients", Description = "per flat test patient", },
                TimeUnit = TimeUnit.PerYear,
                CurrencyCode = "GBP",
                LastUpdated = DateTime.UtcNow,
            },
            new()
            {
                CataloguePriceId = 2,
                CatalogueItemId = new CatalogueItemId(99998, "001"),
                ProvisioningType = ProvisioningType.OnDemand,
                CataloguePriceType = CataloguePriceType.Flat,
                CataloguePriceCalculationType = CataloguePriceCalculationType.Volume,
                PublishedStatus = PublicationStatus.Published,
                CataloguePriceQuantityCalculationType = CataloguePriceQuantityCalculationType.PerServiceRecipient,
                PricingUnit =
                    new PricingUnit { RangeDescription = "Patients", Description = "per flat test on demand", },
                TimeUnit = TimeUnit.PerYear,
                CurrencyCode = "GBP",
                LastUpdated = DateTime.UtcNow,
            },
            new()
            {
                CataloguePriceId = 3,
                CatalogueItemId = new CatalogueItemId(99998, "001"),
                ProvisioningType = ProvisioningType.Declarative,
                CataloguePriceType = CataloguePriceType.Flat,
                CataloguePriceCalculationType = CataloguePriceCalculationType.Volume,
                PublishedStatus = PublicationStatus.Published,
                CataloguePriceQuantityCalculationType = CataloguePriceQuantityCalculationType.PerServiceRecipient,
                PricingUnit =
                    new PricingUnit { RangeDescription = "Patients", Description = "per flat test declarative", },
                TimeUnit = TimeUnit.PerYear,
                CurrencyCode = "GBP",
                LastUpdated = DateTime.UtcNow,
            },
            new()
            {
                CataloguePriceId = 4,
                CatalogueItemId = new CatalogueItemId(99998, "001"),
                ProvisioningType = ProvisioningType.Patient,
                CataloguePriceType = CataloguePriceType.Tiered,
                CataloguePriceCalculationType = CataloguePriceCalculationType.Cumulative,
                PublishedStatus = PublicationStatus.Published,
                PricingUnit =
                    new PricingUnit
                    {
                        RangeDescription = "Patients", Description = "per tiered cumulative test patient",
                    },
                TimeUnit = TimeUnit.PerYear,
                CurrencyCode = "GBP",
                LastUpdated = DateTime.UtcNow,
            },
            new()
            {
                CataloguePriceId = 5,
                CatalogueItemId = new CatalogueItemId(99998, "001"),
                ProvisioningType = ProvisioningType.Patient,
                CataloguePriceType = CataloguePriceType.Tiered,
                CataloguePriceCalculationType = CataloguePriceCalculationType.SingleFixed,
                PublishedStatus = PublicationStatus.Published,
                PricingUnit =
                    new PricingUnit
                    {
                        RangeDescription = "Patients", Description = "per tiered single fixed test patient",
                    },
                TimeUnit = TimeUnit.PerYear,
                CurrencyCode = "GBP",
                LastUpdated = DateTime.UtcNow,
            },
            new()
            {
                CataloguePriceId = 6,
                CatalogueItemId = new CatalogueItemId(99998, "002"),
                ProvisioningType = ProvisioningType.Declarative,
                CataloguePriceType = CataloguePriceType.Flat,
                PublishedStatus = PublicationStatus.Published,
                CataloguePriceCalculationType = CataloguePriceCalculationType.SingleFixed,
                CataloguePriceQuantityCalculationType = CataloguePriceQuantityCalculationType.PerSolutionOrService,
                PricingUnit = new PricingUnit { RangeDescription = "Test Tier", Description = "Per Test", },
                TimeUnit = TimeUnit.PerYear,
                CurrencyCode = "GBP",
                LastUpdated = DateTime.UtcNow,
            },
            new()
            {
                CataloguePriceId = 7,
                CatalogueItemId = new CatalogueItemId(99999, "001"),
                ProvisioningType = ProvisioningType.Patient,
                CataloguePriceType = CataloguePriceType.Flat,
                PublishedStatus = PublicationStatus.Published,
                CataloguePriceCalculationType = CataloguePriceCalculationType.SingleFixed,
                PricingUnit = new PricingUnit { RangeDescription = "Test Tier", Description = "per test patient", },
                TimeUnit = TimeUnit.PerYear,
                CurrencyCode = "GBP",
                LastUpdated = DateTime.UtcNow,
            },
            new()
            {
                CataloguePriceId = 8,
                CatalogueItemId = new CatalogueItemId(99999, "001"),
                ProvisioningType = ProvisioningType.Patient,
                CataloguePriceType = CataloguePriceType.Flat,
                PublishedStatus = PublicationStatus.Draft,
                CataloguePriceCalculationType = CataloguePriceCalculationType.SingleFixed,
                PricingUnit = new PricingUnit { RangeDescription = "Test Tier", Description = "per test patient", },
                TimeUnit = TimeUnit.PerYear,
                CurrencyCode = "GBP",
                LastUpdated = DateTime.UtcNow,
            },
            new()
            {
                CataloguePriceId = 9,
                CatalogueItemId = new CatalogueItemId(99997, "001"),
                ProvisioningType = ProvisioningType.Patient,
                CataloguePriceType = CataloguePriceType.Flat,
                PublishedStatus = PublicationStatus.Published,
                CataloguePriceCalculationType = CataloguePriceCalculationType.SingleFixed,
                PricingUnit = new PricingUnit { RangeDescription = "Test Tier", Description = "per test patient", },
                TimeUnit = TimeUnit.PerYear,
                CurrencyCode = "GBP",
                LastUpdated = DateTime.UtcNow,
            },
            new()
            {
                CataloguePriceId = 10,
                CatalogueItemId = new CatalogueItemId(99998, "001A99"),
                ProvisioningType = ProvisioningType.Patient,
                CataloguePriceType = CataloguePriceType.Flat,
                CataloguePriceCalculationType = CataloguePriceCalculationType.SingleFixed,
                PublishedStatus = PublicationStatus.Published,
                PricingUnit = new PricingUnit { RangeDescription = "Test Tier", Description = "per test patient", },
                TimeUnit = TimeUnit.PerYear,
                CurrencyCode = "GBP",
                LastUpdated = DateTime.UtcNow,
            },
            new()
            {
                CataloguePriceId = 11,
                CatalogueItemId = new CatalogueItemId(99998, "001A99"),
                ProvisioningType = ProvisioningType.OnDemand,
                CataloguePriceType = CataloguePriceType.Flat,
                PublishedStatus = PublicationStatus.Published,
                CataloguePriceCalculationType = CataloguePriceCalculationType.SingleFixed,
                PricingUnit = new PricingUnit { RangeDescription = "Test Tier", Description = "per test on demand", },
                TimeUnit = TimeUnit.PerYear,
                CurrencyCode = "GBP",
                LastUpdated = DateTime.UtcNow,
                CataloguePriceTiers = new List<CataloguePriceTier> {},
            },
            new()
            {
                CataloguePriceId = 12,
                CatalogueItemId = new CatalogueItemId(99998, "001A99"),
                ProvisioningType = ProvisioningType.Declarative,
                CataloguePriceType = CataloguePriceType.Flat,
                CataloguePriceCalculationType = CataloguePriceCalculationType.SingleFixed,
                PublishedStatus = PublicationStatus.Published,
                PricingUnit = new PricingUnit { RangeDescription = "Test Tier", Description = "per test declarative", },
                TimeUnit = TimeUnit.PerYear,
                CurrencyCode = "GBP",
                LastUpdated = DateTime.UtcNow,
                CataloguePriceTiers = new List<CataloguePriceTier> {},
            },
            new()
            {
                CataloguePriceId = 13,
                CatalogueItemId = new CatalogueItemId(99998, "001A99"),
                ProvisioningType = ProvisioningType.Patient,
                CataloguePriceType = CataloguePriceType.Flat,
                CataloguePriceCalculationType = CataloguePriceCalculationType.SingleFixed,
                PublishedStatus = PublicationStatus.Draft,
                PricingUnit = new PricingUnit { RangeDescription = "Test Tier", Description = "per test declarative", },
                TimeUnit = TimeUnit.PerYear,
                CurrencyCode = "GBP",
                LastUpdated = DateTime.UtcNow,
                CataloguePriceTiers = new List<CataloguePriceTier> {},
            },
            new()
            {
                CataloguePriceId = 14,
                CatalogueItemId = new CatalogueItemId(99998, "001A99"),
                ProvisioningType = ProvisioningType.Patient,
                CataloguePriceType = CataloguePriceType.Tiered,
                CataloguePriceCalculationType = CataloguePriceCalculationType.Cumulative,
                PublishedStatus = PublicationStatus.Draft,
                PricingUnit =
                    new PricingUnit { RangeDescription = "Test Tier", Description = "per tiered test declarative", },
                TimeUnit = TimeUnit.PerYear,
                CurrencyCode = "GBP",
                LastUpdated = DateTime.UtcNow,
            },
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
                    RangeDescription = "Test Tier",
                    Description = "per test declarative",
                },
                TimeUnit = TimeUnit.PerYear,
                CurrencyCode = "GBP",
                LastUpdated = DateTime.UtcNow,
            },
            new()
            {
                CataloguePriceId = 16,
                CatalogueItemId = new CatalogueItemId(99998, "S-997"),
                ProvisioningType = ProvisioningType.Patient,
                CataloguePriceType = CataloguePriceType.Flat,
                PublishedStatus = PublicationStatus.Published,
                CataloguePriceCalculationType = CataloguePriceCalculationType.SingleFixed,
                PricingUnit =
                    new PricingUnit { RangeDescription = "Test Tier", Description = "per test patient", },
                TimeUnit = TimeUnit.PerYear,
                CurrencyCode = "GBP",
                LastUpdated = DateTime.UtcNow,
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
                    new PricingUnit { RangeDescription = "Test Tier", Description = "per test on demand", },
                TimeUnit = TimeUnit.PerYear,
                CurrencyCode = "GBP",
                LastUpdated = DateTime.UtcNow,
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
                    new PricingUnit { RangeDescription = "Test Tier", Description = "per test declarative", },
                TimeUnit = TimeUnit.PerYear,
                CurrencyCode = "GBP",
                LastUpdated = DateTime.UtcNow,
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
                    new PricingUnit { RangeDescription = "Test Tier", Description = "per test declarative", },
                TimeUnit = TimeUnit.PerYear,
                CurrencyCode = "GBP",
                LastUpdated = DateTime.UtcNow,
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
                    new PricingUnit { RangeDescription = "Test Tier", Description = "per tiered price", },
                TimeUnit = TimeUnit.PerYear,
                CurrencyCode = "GBP",
                LastUpdated = DateTime.UtcNow,
            },
            new()
            {
                CataloguePriceId = 21,
                CatalogueItemId = new CatalogueItemId(99998, "S-998"),
                ProvisioningType = ProvisioningType.Patient,
                CataloguePriceType = CataloguePriceType.Flat,
                PublishedStatus = PublicationStatus.Published,
                CataloguePriceCalculationType = CataloguePriceCalculationType.SingleFixed,
                PricingUnit =
                    new PricingUnit { RangeDescription = "Test Tier", Description = "per test patient", },
                TimeUnit = TimeUnit.PerYear,
                CurrencyCode = "GBP",
                LastUpdated = DateTime.UtcNow,
            },
            new()
            {
                CataloguePriceId = 22,
                CatalogueItemId = new CatalogueItemId(99998, "S-999"),
                ProvisioningType = ProvisioningType.Patient,
                CataloguePriceType = CataloguePriceType.Flat,
                PublishedStatus = PublicationStatus.Published,
                CataloguePriceCalculationType = CataloguePriceCalculationType.SingleFixed,
                PricingUnit =
                    new PricingUnit { RangeDescription = "Test Tier", Description = "per test patient", },
                TimeUnit = TimeUnit.PerYear,
                CurrencyCode = "GBP",
                LastUpdated = DateTime.UtcNow,
            },
            new()
            {
                CataloguePriceId = 23,
                CatalogueItemId = new CatalogueItemId(99999, "S-999"),
                ProvisioningType = ProvisioningType.Patient,
                CataloguePriceType = CataloguePriceType.Tiered,
                CataloguePriceCalculationType = CataloguePriceCalculationType.Cumulative,
                PublishedStatus = PublicationStatus.Published,
                PricingUnit =
                    new() { RangeDescription = "Test Tier", Description = "per patient", },
                TimeUnit = TimeUnit.PerYear,
                CurrencyCode = "GBP",
                LastUpdated = DateTime.UtcNow,
            },
            new()
            {
                CataloguePriceId = 24,
                CatalogueItemId = new CatalogueItemId(99999, "S-998"),
                ProvisioningType = ProvisioningType.Patient,
                CataloguePriceType = CataloguePriceType.Flat,
                PublishedStatus = PublicationStatus.Published,
                CataloguePriceCalculationType = CataloguePriceCalculationType.SingleFixed,
                PricingUnit =
                    new PricingUnit { RangeDescription = "Test Tier", Description = "per test patient", },
                TimeUnit = TimeUnit.PerYear,
                CurrencyCode = "GBP",
                LastUpdated = DateTime.UtcNow,
            },
            new()
            {
                CataloguePriceId = 25,
                CatalogueItemId = new CatalogueItemId(99999, "001A99"),
                ProvisioningType = ProvisioningType.Patient,
                CataloguePriceType = CataloguePriceType.Tiered,
                CataloguePriceCalculationType = CataloguePriceCalculationType.Cumulative,
                PublishedStatus = PublicationStatus.Published,
                PricingUnit =
                    new PricingUnit { RangeDescription = "Test Tier", Description = "per tiered test declarative", },
                TimeUnit = TimeUnit.PerYear,
                CurrencyCode = "GBP",
                LastUpdated = DateTime.UtcNow,
            },
            new()
            {
                CataloguePriceId = 26,
                CatalogueItemId = new CatalogueItemId(99999, "001A98"),
                ProvisioningType = ProvisioningType.Patient,
                CataloguePriceType = CataloguePriceType.Flat,
                CataloguePriceCalculationType = CataloguePriceCalculationType.SingleFixed,
                PublishedStatus = PublicationStatus.Published,
                PricingUnit = new PricingUnit { RangeDescription = "Test Tier", Description = "per test patient", },
                TimeUnit = TimeUnit.PerYear,
                CurrencyCode = "GBP",
                LastUpdated = DateTime.UtcNow,
            },
            new()
            {
                CataloguePriceId = 27,
                CatalogueItemId = new CatalogueItemId(99998, "001"),
                ProvisioningType = ProvisioningType.Patient,
                CataloguePriceType = CataloguePriceType.Tiered,
                CataloguePriceCalculationType = CataloguePriceCalculationType.Cumulative,
                PublishedStatus = PublicationStatus.Draft,
                PricingUnit =
                    new PricingUnit
                    {
                        RangeDescription = "Patients",
                        Description = "per tiered single fixed test patient Maximum Tiers Unpublished",
                    },
                TimeUnit = TimeUnit.PerYear,
                CurrencyCode = "GBP",
                LastUpdated = DateTime.UtcNow,
            },
            new()
            {
                CataloguePriceId = 28,
                CatalogueItemId = new CatalogueItemId(99998, "001A99"),
                ProvisioningType = ProvisioningType.Patient,
                CataloguePriceType = CataloguePriceType.Tiered,
                CataloguePriceCalculationType = CataloguePriceCalculationType.Cumulative,
                PublishedStatus = PublicationStatus.Draft,
                PricingUnit = new PricingUnit
                {
                    RangeDescription = "Patients",
                    Description = "per tiered single fixed test patient Maximum Tiers Unpublished",
                },
                TimeUnit = TimeUnit.PerYear,
                CurrencyCode = "GBP",
                LastUpdated = DateTime.UtcNow,
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
                    new PricingUnit { RangeDescription = "Test Tier", Description = "maximum tiers", },
                TimeUnit = TimeUnit.PerYear,
                CurrencyCode = "GBP",
                LastUpdated = DateTime.UtcNow,
            },
            new()
            {
                CataloguePriceId = 30,
                CatalogueItemId = new CatalogueItemId(99998, "001A98"),
                ProvisioningType = ProvisioningType.Patient,
                CataloguePriceType = CataloguePriceType.Flat,
                CataloguePriceCalculationType = CataloguePriceCalculationType.SingleFixed,
                PublishedStatus = PublicationStatus.Published,
                PricingUnit = new PricingUnit
                {
                    RangeDescription = "Test Tier",
                    Description = "per test patient",
                },
                TimeUnit = TimeUnit.PerYear,
                CurrencyCode = "GBP",
                LastUpdated = DateTime.UtcNow,
            },
            new()
            {
                CataloguePriceId = 31,
                CatalogueItemId = new CatalogueItemId(99999, "003"),
                ProvisioningType = ProvisioningType.Patient,
                CataloguePriceType = CataloguePriceType.Flat,
                PublishedStatus = PublicationStatus.Published,
                CataloguePriceCalculationType = CataloguePriceCalculationType.SingleFixed,
                PricingUnit = new PricingUnit { RangeDescription = "Test Tier", Description = "per test patient", },
                TimeUnit = TimeUnit.PerYear,
                CurrencyCode = "GBP",
                LastUpdated = DateTime.UtcNow,
            },
        };
    }

    internal static List<CataloguePriceTier> GetCataloguePriceTiers()
    {
        return new List<CataloguePriceTier>
        {
            new()
            {
                Id = -2,
                CataloguePriceId = -2,
                LowerRange = 1,
                UpperRange = null,
                Price = 100.01M,
                LastUpdated = DateTime.UtcNow,
            },
            new()
            {
                Id = -1,
                CataloguePriceId = -1,
                LowerRange = 1,
                UpperRange = null,
                Price = 100.01M,
                LastUpdated = DateTime.UtcNow,
            },
            new()
            {
                Id = 1,
                CataloguePriceId = 1,
                LowerRange = 1,
                UpperRange = null,
                Price = 999.9999M,
                LastUpdated = DateTime.UtcNow,
            },
            new()
            {
                Id = 2,
                CataloguePriceId = 2,
                LowerRange = 1,
                UpperRange = null,
                Price = 999.9999M,
                LastUpdated = DateTime.UtcNow,
            },
            new()
            {
                Id = 3,
                CataloguePriceId = 4,
                LowerRange = 1,
                UpperRange = 89999,
                Price = 999.9999M,
                LastUpdated = DateTime.UtcNow,
            },
            new()
            {
                Id = 4,
                CataloguePriceId = 4,
                LowerRange = 90000,
                UpperRange = 899999,
                LastUpdated = DateTime.UtcNow,
            },
            new()
            {
                Id = 5,
                CataloguePriceId = 4,
                LowerRange = 900000,
                UpperRange = null,
                LastUpdated = DateTime.UtcNow,
            },
            new()
            {
                Id = 6,
                CataloguePriceId = 5,
                LowerRange = 1,
                UpperRange = 89999,
                Price = 999.9999M,
                LastUpdated = DateTime.UtcNow,
            },
            new()
            {
                Id = 7,
                CataloguePriceId = 5,
                LowerRange = 90000,
                UpperRange = 899999,
                LastUpdated = DateTime.UtcNow,
            },
            new()
            {
                Id = 8,
                CataloguePriceId = 5,
                LowerRange = 900000,
                UpperRange = null,
                LastUpdated = DateTime.UtcNow,
            },
            new()
            {
                Id = 9,
                CataloguePriceId = 6,
                LowerRange = 1,
                UpperRange = null,
                Price = 999.9999M,
                LastUpdated = DateTime.UtcNow,
            },
            new()
            {
                Id = 10,
                CataloguePriceId = 7,
                LowerRange = 1,
                UpperRange = null,
                Price = 999.9999M,
                LastUpdated = DateTime.UtcNow,
            },
            new()
            {
                Id = 11,
                CataloguePriceId = 8,
                LowerRange = 1,
                UpperRange = null,
                Price = 999.9999M,
                LastUpdated = DateTime.UtcNow,
            },
            new()
            {
                Id = 12,
                CataloguePriceId = 9,
                LowerRange = 1,
                UpperRange = null,
                Price = 999.9999M,
                LastUpdated = DateTime.UtcNow,
            },
            new()
            {
                Id = 13,
                CataloguePriceId = 10,
                LowerRange = 1,
                UpperRange = null,
                Price = 999.9999M,
                LastUpdated = DateTime.UtcNow,
            },
            new()
            {
                Id = 14,
                CataloguePriceId = 11,
                LowerRange = 1,
                UpperRange = null,
                Price = 999.9999M,
                LastUpdated = DateTime.UtcNow,
            },
            new()
            {
                Id = 15,
                CataloguePriceId = 12,
                LowerRange = 1,
                UpperRange = null,
                Price = 999.9999M,
                LastUpdated = DateTime.UtcNow,
            },
            new()
            {
                Id = 16,
                CataloguePriceId = 13,
                LowerRange = 1,
                UpperRange = null,
                Price = 999.9999M,
                LastUpdated = DateTime.UtcNow,
            },
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
            new()
            {
                Id = 20,
                CataloguePriceId = 15,
                LowerRange = 1,
                UpperRange = null,
                Price = 999.9999M,
                LastUpdated = DateTime.UtcNow,
            },
            new()
            {
                Id = 21,
                CataloguePriceId = 16,
                LowerRange = 1,
                UpperRange = null,
                Price = 999.9999M,
                LastUpdated = DateTime.UtcNow,
            },
            new()
            {
                Id = 22,
                CataloguePriceId = 17,
                LowerRange = 1,
                UpperRange = null,
                Price = 999.9999M,
                LastUpdated = DateTime.UtcNow,
            },
            new()
            {
                Id = 23,
                CataloguePriceId = 18,
                LowerRange = 1,
                UpperRange = null,
                Price = 999.9999M,
                LastUpdated = DateTime.UtcNow,
            },
            new()
            {
                Id = 24,
                CataloguePriceId = 19,
                LowerRange = 1,
                UpperRange = null,
                Price = 999.9999M,
                LastUpdated = DateTime.UtcNow,
            },
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
            new()
            {
                Id = 28,
                CataloguePriceId = 21,
                LowerRange = 1,
                UpperRange = null,
                Price = 999.9999M,
                LastUpdated = DateTime.UtcNow,
            },
            new()
            {
                Id = 29,
                CataloguePriceId = 22,
                LowerRange = 1,
                UpperRange = null,
                Price = 999.9999M,
                LastUpdated = DateTime.UtcNow,
            },
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
            new()
            {
                Id = 33,
                CataloguePriceId = 24,
                LowerRange = 1,
                UpperRange = null,
                Price = 999.9999M,
                LastUpdated = DateTime.UtcNow,
            },
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
            new()
            {
                Id = 37,
                CataloguePriceId = 26,
                LowerRange = 1,
                UpperRange = null,
                Price = 999.9999M,
                LastUpdated = DateTime.UtcNow,
            },
            new()
            {
                Id = 38,
                CataloguePriceId = 27,
                LowerRange = 1,
                UpperRange = 9999,
                Price = 999.9999M,
                LastUpdated = DateTime.UtcNow,
            },
            new()
            {
                Id = 39,
                CataloguePriceId = 27,
                LowerRange = 10000,
                UpperRange = 49000,
                LastUpdated = DateTime.UtcNow,
            },
            new()
            {
                Id = 40,
                CataloguePriceId = 27,
                LowerRange = 50000,
                UpperRange = 99999,
                LastUpdated = DateTime.UtcNow,
            },
            new()
            {
                Id = 41,
                CataloguePriceId = 27,
                LowerRange = 100000,
                UpperRange = 149999,
                LastUpdated = DateTime.UtcNow,
            },
            new()
            {
                Id = 42,
                CataloguePriceId = 27,
                LowerRange = 150000,
                UpperRange = null,
                LastUpdated = DateTime.UtcNow,
            },
            new()
            {
                Id = 43,
                CataloguePriceId = 27,
                LowerRange = 150000,
                UpperRange = null,
                LastUpdated = DateTime.UtcNow,
            },
            new()
            {
                Id = 44,
                CataloguePriceId = 27,
                LowerRange = 150000,
                UpperRange = null,
                LastUpdated = DateTime.UtcNow,
            },
            new()
            {
                Id = 45,
                CataloguePriceId = 28,
                LowerRange = 1,
                UpperRange = 9999,
                Price = 999.9999M,
                LastUpdated = DateTime.UtcNow,
            },
            new()
            {
                Id = 46,
                CataloguePriceId = 28,
                LowerRange = 10000,
                UpperRange = 49000,
                LastUpdated = DateTime.UtcNow,
            },
            new()
            {
                Id = 47,
                CataloguePriceId = 28,
                LowerRange = 50000,
                UpperRange = 99999,
                LastUpdated = DateTime.UtcNow,
            },
            new()
            {
                Id = 48,
                CataloguePriceId = 28,
                LowerRange = 100000,
                UpperRange = 149999,
                LastUpdated = DateTime.UtcNow,
            },
            new()
            {
                Id = 49,
                CataloguePriceId = 28,
                LowerRange = 150000,
                UpperRange = null,
                LastUpdated = DateTime.UtcNow,
            },
            new()
            {
                Id = 50,
                CataloguePriceId = 28,
                LowerRange = 150000,
                UpperRange = null,
                LastUpdated = DateTime.UtcNow,
            },
            new()
            {
                Id = 51,
                CataloguePriceId = 28,
                LowerRange = 150000,
                UpperRange = null,
                LastUpdated = DateTime.UtcNow,
            },
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
            new()
            {
                Id = 59,
                CataloguePriceId = 31,
                LowerRange = 1,
                UpperRange = null,
                Price = 999.9999M,
                LastUpdated = DateTime.UtcNow,
            },
            new()
            {
                Id = 60,
                CataloguePriceId = 3,
                LowerRange = 1,
                UpperRange = null,
                Price = 999.9999M,
                LastUpdated = DateTime.UtcNow,
            },
            new()
            {
                Id = 61,
                CataloguePriceId = 30,
                LowerRange = 1,
                UpperRange = null,
                Price = 999.9999M,
                LastUpdated = DateTime.UtcNow,
            },
        };
    }
}
