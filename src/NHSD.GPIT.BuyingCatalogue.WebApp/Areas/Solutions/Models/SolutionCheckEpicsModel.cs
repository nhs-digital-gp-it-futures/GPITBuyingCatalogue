using System;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public sealed class SolutionCheckEpicsModel : NavBaseModel, INoNavModel
    {
        public SolutionCheckEpicsModel()
        {
        }

        public SolutionCheckEpicsModel(
            CatalogueItemCapability solutionCapability,
            CatalogueItem catalogueItem)
        {
            Description = solutionCapability.Capability?.Description;
            Name = solutionCapability.Capability?.Name;
            SolutionId = solutionCapability.CatalogueItemId;
            NhsDefined = GetEpics(catalogueItem, false);
            SupplierDefined = GetEpics(catalogueItem, true);
            LastReviewed = solutionCapability.LastUpdated;
            SolutionName = catalogueItem.Name;
            SolutionId = catalogueItem.Id;
            Url = solutionCapability.Capability.SourceUrl;
        }

        public SolutionCheckEpicsModel(
            CatalogueItemCapability solutionCapability,
            CatalogueItem catalogueItem,
            CatalogueItemId additionalServiceId)
            : this(solutionCapability, catalogueItem)
        {
            CatalogueItemIdAdditional = additionalServiceId;
        }

        public CatalogueItemId CatalogueItemIdAdditional { get; init; }

        public string Description { get; init; }

        public DateTime LastReviewed { get; set; }

        public string Name { get; set; }

        public string Url { get; init; }

        public string[] NhsDefined { get; init; }

        public CatalogueItemId SolutionId { get; set; }

        public string SolutionName { get; set; }

        public string[] SupplierDefined { get; init; }

        public bool HasNhsDefined() => NhsDefined != null && NhsDefined.Length > 0;

        public bool HasNoEpics() => !HasNhsDefined() && !HasSupplierDefined();

        public bool HasSupplierDefined() => SupplierDefined != null && SupplierDefined.Length > 0;

        private static string[] GetEpics(CatalogueItem item, bool supplierDefined) =>
            item.CatalogueItemEpics?
                .Where(e => e.Epic.SupplierDefined == supplierDefined)
                .Select(epic => epic.Epic.Name)
                .OrderBy(name => name)
                .ToArray();
    }
}
