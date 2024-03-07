using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Notifications.Models
{
    public class BuyerOrderCompletedEmailContent() : GovNotifyEmailContent(NotificationTypeEnum.BuyerOrderCompleted)
    {
        private const string OrderIdToken = "order_id";
        private const string OrderSummaryCsv = "order_summary_csv";
        private const string UserAssociatedServiceTemplateId = "bd70f6c0-9ff4-4e07-b6a5-22eaecb92b9a";
        private const string UserAmendTemplateId = "658998ab-1711-4c5b-87a5-a2b7d401a23e";
        private const string UserTemplateId = "8bae2d80-19dc-4cb6-b126-3b1a2ee05766";

        public string CallOffId { get; set; }

        public bool IsAmendment { get; set; }

        public OrderTypeEnum OrderType { get; set; }

        public string CsvAttachmentBase64 { get; set; }

        public override Dictionary<string, dynamic> GetTemplatePersonalisation(Func<byte[], bool, JObject> prepareUpload)
        {
            return new Dictionary<string, dynamic>
            {
                { OrderIdToken, $"{CallOffId}" },
                { OrderSummaryCsv, prepareUpload(Convert.FromBase64String(CsvAttachmentBase64), true) },
            };
        }

        public override string GetTemplateId()
        {
            OrderType orderType = OrderType;
            return orderType.AssociatedServicesOnly
                    ? UserAssociatedServiceTemplateId
                    : IsAmendment
                        ? UserAmendTemplateId
                        : UserTemplateId;
        }
    }
}
