using System;
using System.Globalization;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Constants;

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
                        deliveryDate = DateTime.ParseExact($"{Day}/{Month}/{Year}", "d/M/yyyy", CultureInfo.InvariantCulture);
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

        public string ValidateDeliveryDate(DateTime? commencementDate)
        {
            if (!DeliveryDate.HasValue)
                return "Planned delivery date must be a real date";

            if (DeliveryDate.Value.ToUniversalTime() <= DateTime.UtcNow)
                return "Planned delivery date must be in the future";

            if (commencementDate.HasValue && DeliveryDate.Value.ToUniversalTime() > commencementDate.Value.AddMonths(ValidationConstants.MaxDeliveryMonthsFromCommencement))
                return $"Planned delivery date must be within {ValidationConstants.MaxDeliveryMonthsFromCommencement} months from the commencement date for this Call-off Agreement";

            return null;
        }
    }
}
