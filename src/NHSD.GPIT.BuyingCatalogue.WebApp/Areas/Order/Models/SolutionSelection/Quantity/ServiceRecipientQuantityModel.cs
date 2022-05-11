namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.SolutionSelection.Quantity
{
    public class ServiceRecipientQuantityModel
    {
        public string Name { get; set; }

        public string OdsCode { get; set; }

        public string InputQuantity { get; set; }

        public int Quantity { get; set; }

        public string Description => $"{Name} ({OdsCode})";
    }
}
