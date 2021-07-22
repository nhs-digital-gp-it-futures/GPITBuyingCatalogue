namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models
{
    public sealed class HostingType
    {
        public HostingType(string name, string id)
        {
            Name = name;
            Id = id;
        }

        public string Name { get; set; }

        public string Id { get; set; }
    }
}
