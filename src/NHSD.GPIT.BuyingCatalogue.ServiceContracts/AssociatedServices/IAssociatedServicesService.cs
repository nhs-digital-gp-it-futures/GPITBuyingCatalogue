﻿using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.AssociatedServices;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.AssociatedServices
{
    public interface IAssociatedServicesService
    {
        Task<List<CatalogueItem>> GetAssociatedServicesForSupplier(int? supplierId);

        Task<CatalogueItem> GetAssociatedService(CatalogueItemId associatedServiceId);

        Task<bool> AssociatedServiceExistsWithNameForSupplier(
            string additionalServiceName,
            int supplierId,
            CatalogueItemId currentCatalogueItemId = default);

        Task RelateAssociatedServicesToSolution(CatalogueItemId solutionId, IEnumerable<CatalogueItemId> associatedServices);

        Task EditDetails(CatalogueItemId associatedServiceId, AssociatedServicesDetailsModel model);

        Task<CatalogueItemId> AddAssociatedService(CatalogueItem solution, AssociatedServicesDetailsModel model);

        Task SavePublicationStatus(CatalogueItemId associatedServiceId, PublicationStatus publicationStatus);
    }
}
