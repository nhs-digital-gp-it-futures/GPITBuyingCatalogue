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

        public ServiceRecipientDto(
            string odsCode,
            string name,
            int? quantity,
            string location)
            : this(odsCode, name, quantity)
        {
            Location = location;
        }

        public string OdsCode { get; set; }

        public string Name { get; set; }

        public int? Quantity { get; set; }

        public string Location { get; set; }
    }
}
