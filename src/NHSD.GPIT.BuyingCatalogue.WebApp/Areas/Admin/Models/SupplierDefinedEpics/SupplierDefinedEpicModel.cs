using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.SupplierDefinedEpics
{
    public struct SupplierDefinedEpicModel
    {
        public SupplierDefinedEpicModel(Epic epic)
        {
            Id = epic.Id;
            Name = epic.Name;
            Capability = epic.Capabilities.FirstOrDefault()?.Name;
            IsActive = epic.IsActive;
        }

        public string Id { get; }

        public string Name { get; }

        public string Capability { get; }

        public bool IsActive { get; }
    }
}
