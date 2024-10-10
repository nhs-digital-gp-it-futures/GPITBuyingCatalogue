namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.Quantities
{
    public class SubLocationModel
    {
        public SubLocationModel()
        {
        }

        public SubLocationModel(string name, ServiceRecipientQuantityModel[] serviceRecipients)
        {
            Name = name;
            ServiceRecipients = serviceRecipients;
        }

        public string Name { get; set; }

        public ServiceRecipientQuantityModel[] ServiceRecipients { get; set; }
    }
}
