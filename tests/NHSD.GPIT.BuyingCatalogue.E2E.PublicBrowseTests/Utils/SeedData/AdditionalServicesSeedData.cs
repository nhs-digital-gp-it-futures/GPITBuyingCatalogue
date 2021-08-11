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
            return new()
            {
                new CatalogueItem
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
                        new CatalogueItemCapability
                        {
                            CatalogueItemId = new CatalogueItemId(99999, "001A999"),
                            CapabilityId = new Guid("4F09E77B-E3A3-4A25-8EC1-815921F83628"),
                            LastUpdated = DateTime.UtcNow,
                            LastUpdatedBy = Guid.Empty,
                            StatusId = 1,
                        },
                    },
                    CatalogueItemEpics = new List<CatalogueItemEpic>
                    {
                        new CatalogueItemEpic
                        {
                            CatalogueItemId = new CatalogueItemId(99999, "001A999"),
                            CapabilityId = new Guid("4F09E77B-E3A3-4A25-8EC1-815921F83628"),
                            EpicId = "C2E1",
                            StatusId = 1,
                        },
                        new CatalogueItemEpic
                        {
                            CatalogueItemId = new CatalogueItemId(99999, "001A999"),
                            CapabilityId = new Guid("4F09E77B-E3A3-4A25-8EC1-815921F83628"),
                            EpicId = "C2E2",
                            StatusId = 1,
                        },
                        new CatalogueItemEpic
                        {
                            CatalogueItemId = new CatalogueItemId(99999, "001A999"),
                            CapabilityId = new Guid("4F09E77B-E3A3-4A25-8EC1-815921F83628"),
                            EpicId = "E123456",
                            StatusId = 1,
                        },
                    },
                },
            };
        }
    }
}
