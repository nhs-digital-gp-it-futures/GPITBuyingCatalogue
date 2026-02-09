namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders
{
    public class OrderItemRecipientQuantityDto
    {
        public string ParentSublocationOdsCode { get; set; }

        public string RecipientOdsCode { get; set; }

        public int? Quantity { get; set; }
    }
}
