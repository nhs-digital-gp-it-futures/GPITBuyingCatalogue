using System;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models
{
    public class OrderItemRecipientModel
    {
        private DateTime? deliveryDate;

        public OrderItemRecipientModel()
        {
        }

        public OrderItemRecipientModel(ServiceRecipient recipient)
        {
            Name = recipient.Name;
            OdsCode = recipient.OrgId;
        }

        public bool Selected { get; set; }

        public string Day { get; set; }

        public string Month { get; set; }

        public string Year { get; set; }

        public DateTime? DeliveryDate
        {
            get
            {
                return deliveryDate;
            }

            set
            {
                deliveryDate = value;

                if (deliveryDate.HasValue)
                {
                    Day = deliveryDate.Value.Day.ToString("00");
                    Month = deliveryDate.Value.Month.ToString("00");
                    Year = deliveryDate.Value.Year.ToString("00");
                }
            }
        }

        public string Name { get; init; }

        public string OdsCode { get; init; }

        public int? Quantity { get; set; }

        public decimal? CostPerYear { get; init; }

        public (DateTime? Date, string Error) ToDateTime(DateTime? commencementDate)
        {
            try
            {
                var date = DateTime.Parse($"{Day}/{Month}/{Year}");

                if (date.ToUniversalTime() <= DateTime.UtcNow)
                    return (null, "Planned delivery date must be in the future");

                if (commencementDate.HasValue && date.ToUniversalTime() > commencementDate.Value.AddMonths(42))
                    return (null, "Planned delivery date must be within 42 months from the commencement date for this Call-off Agreement");

                return (date, null);
            }
            catch (FormatException)
            {
                return (null, "Planned delivery date must be a real date");
            }
        }
    }
}
