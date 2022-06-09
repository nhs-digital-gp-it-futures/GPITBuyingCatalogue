using System;
using System.Globalization;

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
                if (!deliveryDate.HasValue)
                {
                    try
                    {
                        deliveryDate = Day is null || Month is null || Year is null
                                            ? null
                                            : DateTime.ParseExact($"{Day}/{Month}/{Year}", "d/M/yyyy", CultureInfo.InvariantCulture);
                    }
                    catch (FormatException)
                    {
                    }
                }

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

        public bool IsComplete => Quantity.HasValue && DeliveryDate.HasValue;
    }
}
