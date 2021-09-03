using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public class SolutionCheckEpicsModel : INoNavModel
    {
        public CatalogueItemId CatalogueItemIdAdditional { get; set; }

        public string Description { get; set; }

        public DateTime LastReviewed { get; set; }

        public string Name { get; set; }

        public string[] NhsDefined { get; set; }

        public CatalogueItemId SolutionId { get; set; }

        public string SolutionName { get; set; }

        public string[] SupplierDefined { get; set; }

        public virtual bool HasNhsDefined() => NhsDefined != null && NhsDefined.Length > 0;

        public bool HasNoEpics() => !HasNhsDefined() && !HasSupplierDefined();

        public virtual bool HasSupplierDefined() => SupplierDefined != null && SupplierDefined.Length > 0;

        public virtual SolutionCheckEpicsModel WithItems(
            CatalogueItemId catalogueItemId,
            CatalogueItemId catalogueItemIdAdditional,
            string solutionName)
        {
            SolutionId = catalogueItemId;
            CatalogueItemIdAdditional = catalogueItemIdAdditional;

            return WithSolutionName(solutionName);
        }

        public virtual SolutionCheckEpicsModel WithSolutionName(string solutionName)
        {
            if (!string.IsNullOrWhiteSpace(solutionName))
                SolutionName = solutionName;

            return this;
        }
    }
}
