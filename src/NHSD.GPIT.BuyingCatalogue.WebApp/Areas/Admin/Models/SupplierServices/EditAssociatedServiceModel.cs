using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.SupplierServices
{
    public sealed class EditAssociatedServiceModel : NavBaseModel
    {
        public EditAssociatedServiceModel()
        {
        }

        public EditAssociatedServiceModel(Supplier supplier, CatalogueItem associatedService)
            : this()
        {
            SupplierId = supplier.Id;
            SupplierName = supplier.Name;
            AssociatedServiceId = associatedService.Id;
            AssociatedServiceName = associatedService.Name;
            SelectedPublicationStatus = associatedService.PublishedStatus;
            AssociatedServicePublicationStatus = associatedService.PublishedStatus;

            DetailsStatus = (!string.IsNullOrEmpty(associatedService.AssociatedService.Description)
                && !string.IsNullOrEmpty(associatedService.AssociatedService.OrderGuidance)
                && !string.IsNullOrEmpty(associatedService.Name))
                ? TaskProgress.Completed
                : TaskProgress.NotStarted;

            ListPriceStatus =
                associatedService.CataloguePrices.Any(cp => cp.PublishedStatus == PublicationStatus.Published)
                    ? TaskProgress.Completed
                    : associatedService.CataloguePrices.Count != 0
                        ? TaskProgress.InProgress
                        : TaskProgress.NotStarted;
        }

        public EditAssociatedServiceModel(
            Supplier supplier,
            CatalogueItem associatedService,
            IList<CatalogueItem> relatedCatalogueItems)
            : this(supplier, associatedService)
        {
            WithSolutions(relatedCatalogueItems);
            WithAdditionalServices(relatedCatalogueItems);
        }

        public int SupplierId { get; init; }

        public CatalogueItemId AssociatedServiceId { get; init; }

        public string AssociatedServiceName { get; init; }

        public string SupplierName { get; init; }

        public PublicationStatus AssociatedServicePublicationStatus { get; set; }

        public PublicationStatus SelectedPublicationStatus { get; set; }

        public IReadOnlyList<SelectOption<string>> PublicationStatuses => AssociatedServicePublicationStatus
            .GetAvailablePublicationStatuses(CatalogueItemType.AssociatedService)
            .Select(p => new SelectOption<string>(p.Description(), p.EnumMemberName()))
            .ToList();

        public TaskProgress DetailsStatus { get; init; }

        public TaskProgress ListPriceStatus { get; init; }

        public IList<AdditionalService> RelatedAdditionalServices { get; set; }

        public IList<Solution> RelatedSolutions { get; set; }

        public EditAssociatedServiceModel WithSolutions(ICollection<CatalogueItem> catalogueItems)
        {
            RelatedSolutions = catalogueItems
                .Where(x => x.CatalogueItemType == CatalogueItemType.Solution)
                .Select(x => x.Solution)
                .ToList();

            return this;
        }

        public EditAssociatedServiceModel WithAdditionalServices(IList<CatalogueItem> catalogueItems)
        {
            RelatedAdditionalServices = catalogueItems
                .Where(x => x.CatalogueItemType == CatalogueItemType.AdditionalService)
                .Select(x => x.AdditionalService)
                .ToList();

            return this;
        }
    }
}
