using System;
using System.Collections.Generic;
using System.Linq;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Shared
{
    public class DeliveryDatesReviewItemTableModel
    {
        public DeliveryDatesReviewItemTableModel()
        {
        }

        public DeliveryDatesReviewItemTableModel(
            string labelText,
            string editDatesUrl,
            IEnumerable<(string OdsCode, string Name, DateTime? DeliveryDate)> recipients,
            bool showPlannedDeliveryDate = true)
        {
            LabelText = labelText;
            EditDatesUrl = editDatesUrl;
            Recipients = recipients ?? Enumerable.Empty<(string OdsCode, string Name, DateTime? DeliveryDate)>();
            ShowPlannedDeliveryDate = showPlannedDeliveryDate;
        }

        public string LabelText { get; set; }

        public string EditDatesUrl { get; set; }

        public IEnumerable<(string OdsCode, string Name, DateTime? DeliveryDate)> Recipients { get; set; } =
            Enumerable.Empty<(string OdsCode, string Name, DateTime? DeliveryDate)>();

        public bool ShowPlannedDeliveryDate { get; set; } = true;
    }
}
