using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Notifications.Models
{
    public class FinanceOrderCompletedEmailContent() : GovNotifyEmailContent(NotificationTypeEnum.FinanceOrderCompleted)
    {
        public string OrderingParty { get; set; }

        public string CsvAttachmentBase64 { get; set; }

        public override Dictionary<string, dynamic> GetTemplatePersonalisation(Func<byte[], bool, JObject> prepareUpload)
        {
            throw new NotImplementedException();
        }

        public override string GetTemplateId()
        {
            throw new NotImplementedException();
        }
    }
}
