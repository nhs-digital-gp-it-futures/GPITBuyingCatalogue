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
                    Id = new CatalogueItemId(99999, "001A999"),
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
                        LastUpdatedBy = Guid.Empty,
                        Solution = catalogueSolutions.Single(s => s.Id == new CatalogueItemId(99999, "001")).Solution,
                    },
                    CatalogueItemCapabilities = new List<CatalogueItemCapability>
                    {
                        new()
                        {
                            CatalogueItemId = new CatalogueItemId(99999, "001A999"),
                            CapabilityId = 2,
                            LastUpdated = DateTime.UtcNow,
                            LastUpdatedBy = Guid.Empty,
                            StatusId = 1,
                        },
                    },
                    CatalogueItemEpics = new List<CatalogueItemEpic>
                    {
                        new()
                        {
                            CatalogueItemId = new CatalogueItemId(99999, "001A999"),
                            CapabilityId = 2,
                            EpicId = "C2E1",
                            StatusId = 1,
                        },
                        new()
                        {
                            CatalogueItemId = new CatalogueItemId(99999, "001A999"),
                            CapabilityId = 2,
                            EpicId = "C2E2",
                            StatusId = 1,
                        },
                        new()
                        {
                            CatalogueItemId = new CatalogueItemId(99999, "001A999"),
                            CapabilityId = 2,
                            EpicId = "E123456",
                            StatusId = 1,
                        },
                    },
                },
            };
        }
    }
}
