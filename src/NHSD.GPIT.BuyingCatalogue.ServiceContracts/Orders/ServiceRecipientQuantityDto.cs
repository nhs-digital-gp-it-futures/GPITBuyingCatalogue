namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders
{
    public class ServiceRecipientQuantityDto
    {
        public ServiceRecipientQuantityDto()
        {
        }

        public ServiceRecipientQuantityDto(
            string parentSublocationOdsCode,
            string recipientOdsCode,
            string name,
            int? quantity)
        {
            ParentSublocationOdsCode = parentSublocationOdsCode;
            RecipientOdsCode = recipientOdsCode;
            Name = name;
            Quantity = quantity;
        }

        public ServiceRecipientQuantityDto(
            string parentSublocationOdsCode,
            string recipientOdsCode,
            string name,
            int? quantity,
            string location)
            : this(parentSublocationOdsCode, recipientOdsCode, name, quantity)
        {
            Location = location;
        }

        public string ParentSublocationOdsCode { get; set; }

        public string RecipientOdsCode { get; set; }

        public string Name { get; set; }

        public int? Quantity { get; set; }

        public string Location { get; set; }
    }
}
