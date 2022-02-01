namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.SupplierDefinedEpics
{
    public sealed class AddEditSupplierDefinedEpic
    {
        public AddEditSupplierDefinedEpic(
            int capabilityId,
            string name,
            string description,
            bool isActive)
        {
            CapabilityId = capabilityId;
            Name = name;
            Description = description;
            IsActive = isActive;
        }

        public AddEditSupplierDefinedEpic(
            string id,
            int capabilityId,
            string name,
            string description,
            bool isActive)
            : this(
                  capabilityId,
                  name,
                  description,
                  isActive)
        {
            Id = id;
        }

        public string Id { get; set; }

        public int CapabilityId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }
    }
}
