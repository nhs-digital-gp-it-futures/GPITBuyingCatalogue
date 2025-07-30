namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.Quantities
{
    public class ServiceRecipientQuantityModel
    {
        public string Name { get; set; }

        public string ParentSublocationOdsCode { get; set; }

        public string RecipientOdsCode { get; set; }

        public string InputQuantity { get; set; }

        public int Quantity { get; set; }

        public string Description => $"{Name} ({RecipientOdsCode})";
    }
}
