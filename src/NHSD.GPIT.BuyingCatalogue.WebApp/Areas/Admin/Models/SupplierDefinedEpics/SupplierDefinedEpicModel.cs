namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.SupplierDefinedEpics
{
    public struct SupplierDefinedEpicModel
    {
        public SupplierDefinedEpicModel(
            string id,
            string name,
            string capability,
            bool isActive)
        {
            Id = id;
            Name = name;
            Capability = capability;
            IsActive = isActive;
        }

        public string Id { get; }

        public string Name { get; }

        public string Capability { get; }

        public bool IsActive { get; }
    }
}
