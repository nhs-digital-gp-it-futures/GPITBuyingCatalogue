﻿using System;
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
                        Solution = catalogueSolutions.First(s => s.Id == new CatalogueItemId(99999, "001")).Solution,
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
                        Solution = catalogueSolutions.First(s => s.Id == new CatalogueItemId(99999, "001")).Solution,
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
                        Solution = catalogueSolutions.First(s => s.Id == new CatalogueItemId(99998, "001")).Solution,
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
                },
                new()
                {
                    Id = new CatalogueItemId(99998, "001A98"),
                    Name = "E2E Single Price Additional Service",
                    CatalogueItemType = CatalogueItemType.AdditionalService,
                    Created = DateTime.UtcNow,
                    PublishedStatus = PublicationStatus.Published,
                    SupplierId = 99998,
                    AdditionalService = new AdditionalService
                    {
                        Summary = "This is the summary of the Additional Service",
                        FullDescription = "This is the description of the Additional Service",
                        LastUpdated = DateTime.UtcNow,
                        Solution = catalogueSolutions.First(s => s.Id == new CatalogueItemId(99998, "001")).Solution,
                    },
                    CatalogueItemCapabilities = new List<CatalogueItemCapability>
                    {
                        new()
                        {
                            CatalogueItemId = new CatalogueItemId(99998, "001A98"),
                            CapabilityId = 2,
                            LastUpdated = DateTime.UtcNow,
                            StatusId = 1,
                        },
                    },
                    CatalogueItemEpics = new List<CatalogueItemEpic>
                    {
                        new()
                        {
                            CatalogueItemId = new CatalogueItemId(99998, "001A98"),
                            CapabilityId = 27,
                            EpicId = "C27E4",
                            StatusId = 1,
                        },
                        new()
                        {
                            CatalogueItemId = new CatalogueItemId(99998, "001A98"),
                            CapabilityId = 27,
                            EpicId = "C27E5",
                            StatusId = 1,
                        },
                        new()
                        {
                            CatalogueItemId = new CatalogueItemId(99998, "001A98"),
                            CapabilityId = 27,
                            EpicId = "C27E6",
                            StatusId = 1,
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
                        Solution = catalogueSolutions.First(s => s.Id == new CatalogueItemId(99998, "002")).Solution,
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
                },
            };
        }
    }
}
