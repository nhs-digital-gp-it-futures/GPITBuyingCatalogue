namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public class IntegrationModel
    {
        public string Name { get; set; }

        public IntegrationTableModel[] Tables { get; set; }

        public string Title() => $"{Name} {(Tables?.Length > 1 ? "integrations" : "integration")}";
    }
}
