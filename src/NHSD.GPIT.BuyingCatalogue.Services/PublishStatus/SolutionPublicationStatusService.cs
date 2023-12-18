using System;
using System.Linq;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.AdditionalServices;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.AssociatedServices;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.PublishStatus;

namespace NHSD.GPIT.BuyingCatalogue.Services.PublishStatus;

public class SolutionPublicationStatusService : ISolutionPublicationStatusService
{
    private readonly IAdditionalServicesService additionalServicesService;
    private readonly IAssociatedServicesService associatedServicesService;
    private readonly IPublicationStatusService publicationStatusService;

    public SolutionPublicationStatusService(
        IAdditionalServicesService additionalServicesService,
        IAssociatedServicesService associatedServicesService,
        IPublicationStatusService publicationStatusService)
    {
        this.additionalServicesService = additionalServicesService
            ?? throw new ArgumentNullException(nameof(additionalServicesService));
        this.associatedServicesService = associatedServicesService
            ?? throw new ArgumentNullException(nameof(associatedServicesService));
        this.publicationStatusService = publicationStatusService
            ?? throw new ArgumentNullException(nameof(publicationStatusService));
    }

    public async Task SetPublicationStatus(CatalogueItemId catalogueItemId, PublicationStatus publicationStatus)
    {
        await publicationStatusService.SetPublicationStatus(catalogueItemId, publicationStatus);

        if (publicationStatus is not PublicationStatus.Unpublished) return;

        await UnpublishAdditionalServicesAsync(catalogueItemId);

        await UnpublishStaleAssociatedServicesAsync(catalogueItemId);
    }

    private async Task UnpublishAdditionalServicesAsync(CatalogueItemId catalogueItemId)
    {
        var additionalServices = await additionalServicesService.GetAdditionalServicesBySolutionId(catalogueItemId, true);
        foreach (var additionalService in additionalServices)
        {
            await publicationStatusService.SetPublicationStatus(additionalService.Id, PublicationStatus.Unpublished);
        }
    }

    private async Task UnpublishStaleAssociatedServicesAsync(CatalogueItemId catalogueItemId)
    {
        var associatedServices =
            await associatedServicesService.GetPublishedAssociatedServicesForSolution(catalogueItemId);

        foreach (var associatedService in associatedServices)
        {
            var referencedSolutions =
                await associatedServicesService.GetAllSolutionsForAssociatedService(associatedService.Id);
            if (referencedSolutions.Any(x => x.PublishedStatus != PublicationStatus.Unpublished)) continue;

            await publicationStatusService.SetPublicationStatus(associatedService.Id, PublicationStatus.Unpublished);
            await associatedServicesService.RemoveServiceFromSolution(catalogueItemId, associatedService.Id);
        }
    }
}
