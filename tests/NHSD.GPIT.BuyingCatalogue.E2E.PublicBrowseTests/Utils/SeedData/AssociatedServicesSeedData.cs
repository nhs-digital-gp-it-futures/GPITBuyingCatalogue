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
                    AssociatedService =
                        new AssociatedService
                        {
                            Description = "This is the description of the Associated Service",
                            OrderGuidance = "This is how to order",
                            LastUpdated = DateTime.UtcNow,
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
                },
            };
        }
    }
}
