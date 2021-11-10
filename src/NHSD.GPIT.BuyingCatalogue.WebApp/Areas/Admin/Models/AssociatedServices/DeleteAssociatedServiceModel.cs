﻿using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.AssociatedServices
{
    public sealed class DeleteAssociatedServiceModel : NavBaseModel
    {
        public DeleteAssociatedServiceModel()
        {
        }

        public DeleteAssociatedServiceModel(CatalogueItemId solutionId, CatalogueItem associatedService)
        {
            if (associatedService is null)
                throw new ArgumentNullException(nameof(associatedService));

            BackLink = $"/admin/catalogue-solutions/manage/{solutionId}/associated-services/{associatedService.Id}/edit-associated-service";
            AssociatedService = associatedService;
        }

        public CatalogueItem AssociatedService { get; }
    }
}
