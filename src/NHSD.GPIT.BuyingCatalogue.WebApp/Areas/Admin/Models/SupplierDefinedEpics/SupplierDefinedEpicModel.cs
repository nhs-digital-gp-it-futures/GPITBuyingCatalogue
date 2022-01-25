namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.SupplierDefinedEpics
{
    public struct SupplierDefinedEpicModel
    {
        public SupplierDefinedEpicModel(
            string id,
            string name,
            string capability)
        {
            Id = id;
            Name = name;
            Capability = capability;
        }

        public string Id { get; }

        public string Capability { get; }

        public string Name { get; }
    }
}
