using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.PublishStatus;

namespace NHSD.GPIT.BuyingCatalogue.Services.PublishStatus
{
    public class PublicationStatusService : IPublicationStatusService
    {
        private readonly BuyingCatalogueDbContext context;

        public PublicationStatusService(BuyingCatalogueDbContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task SetPublicationStatus(CatalogueItemId catalogueItemId, PublicationStatus publicationStatus)
        {
            var catalogueItem = await context.CatalogueItems.FirstAsync(ci => ci.Id == catalogueItemId);
            if (catalogueItem.PublishedStatus == publicationStatus)
                return;

            if (publicationStatus == PublicationStatus.Published)
                catalogueItem.LastPublished = DateTime.UtcNow;

            catalogueItem.PublishedStatus = publicationStatus;

            await context.SaveChangesAsync();
        }
    }
}
