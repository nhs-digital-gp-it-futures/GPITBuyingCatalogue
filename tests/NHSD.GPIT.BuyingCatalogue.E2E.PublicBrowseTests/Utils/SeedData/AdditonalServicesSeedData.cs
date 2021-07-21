using System;
using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.SeedData
{
    public static class AdditonalServicesSeedData
    {
        public static List<CatalogueItem> GetAdditionalServices()
        {
            return new()
            {
                new CatalogueItem
                {
                    CatalogueItemId = new CatalogueItemId(99999, "001A999"),
                    Name = "Additional service",
                    CatalogueItemType = CatalogueItemType.AdditionalService,
                    Created = DateTime.UtcNow,
                    PublishedStatus = PublicationStatus.Published,
                    SupplierId = "99999",
                    AdditionalService = new AdditionalService
                    {
                        Summary = "This is the summary of the Additional Service",
                        FullDescription = "This is the description of the Additional Service",
                        LastUpdated = DateTime.UtcNow,
                        LastUpdatedBy = Guid.Empty,
                        Solution = catalogueSolutions.Single(s => s.CatalogueItemId == new CatalogueItemId(99999, "001")).Solution,
                    },
                },
            }
    }
    }
