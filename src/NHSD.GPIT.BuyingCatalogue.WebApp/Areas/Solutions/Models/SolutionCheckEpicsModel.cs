using System;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public sealed class SolutionCheckEpicsModel : NavBaseModel, INoNavModel
    {
        public SolutionCheckEpicsModel()
        {
        }

        public SolutionCheckEpicsModel(CatalogueItemCapability solutionCapability)
        {
            Description = solutionCapability.Capability?.Description;
            Name = solutionCapability.Capability?.Name;
            SolutionId = solutionCapability.CatalogueItemId;
            NhsDefined = GetEpics(solutionCapability.Capability, false);
            SupplierDefined = GetEpics(solutionCapability.Capability, true);
            LastReviewed = solutionCapability.LastUpdated;
        }

        public CatalogueItemId CatalogueItemIdAdditional { get; set; }

        public string Description { get; set; }

        public DateTime LastReviewed { get; set; }

        public string Name { get; set; }

        public string[] NhsDefined { get; set; }

        public CatalogueItemId SolutionId { get; set; }

        public string SolutionName { get; set; }

        public string[] SupplierDefined { get; set; }

        public bool HasNhsDefined() => NhsDefined != null && NhsDefined.Length > 0;

        public bool HasNoEpics() => !HasNhsDefined() && !HasSupplierDefined();

        public bool HasSupplierDefined() => SupplierDefined != null && SupplierDefined.Length > 0;

        private static string[] GetEpics(Capability capability, bool supplierDefined) =>
            capability?
                .Epics?
                .Where(e => e.IsActive && e.SupplierDefined == supplierDefined)
                .Select(epic => epic.Name)
                .OrderBy(name => name)
                .ToArray();
    }
}
