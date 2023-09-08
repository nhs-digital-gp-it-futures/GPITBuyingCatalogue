namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders
{
    public class ServiceRecipientDto
    {
        public ServiceRecipientDto()
        {
        }

        public ServiceRecipientDto(
            string odsCode,
            string name,
            int? quantity)
        {
            OdsCode = odsCode;
            Name = name;
            Quantity = quantity;
        }

        public string OdsCode { get; set; }

        public string Name { get; set; }

        public int? Quantity { get; set; }
    }
}
