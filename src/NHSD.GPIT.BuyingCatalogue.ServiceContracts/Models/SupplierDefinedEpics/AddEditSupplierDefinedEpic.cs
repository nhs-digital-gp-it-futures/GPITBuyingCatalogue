using System.Collections.Generic;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.SupplierDefinedEpics
{
    public sealed class AddEditSupplierDefinedEpic
    {
        public AddEditSupplierDefinedEpic(
            List<int> capabilityIds,
            string name,
            string description,
            bool isActive)
        {
            CapabilityIds = capabilityIds;
            Name = name;
            Description = description;
            IsActive = isActive;
        }

        public AddEditSupplierDefinedEpic(
            string id,
            List<int> capabilityIds,
            string name,
            string description,
            bool isActive)
            : this(
                  capabilityIds,
                  name,
                  description,
                  isActive)
        {
            Id = id;
        }

        public string Id { get; set; }

        public List<int> CapabilityIds { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }
    }
}
