using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Notifications.Models
{
    public class ContractDueToExpireEmailModel() : GovNotifyEmailModel(EmailNotificationTypeEnum.ContractDueToExpire)
    {
        public const string OrderIdToken = "order_id";
        public const string FirstNameToken = "first_name";
        public const string LastNameToken = "last_name";
        public const string DaysRemainingToken = "number_of_days";

        public string CallOffId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int DaysRemaining { get; set; }

        public override Dictionary<string, dynamic> GetTemplatePersonalisation(Func<byte[], bool, JObject> prepareUpload)
        {
            return new Dictionary<string, dynamic>
            {
                { OrderIdToken, $"{CallOffId}" },
                { FirstNameToken, $"{FirstName}" },
                { LastNameToken, $"{LastName}" },
                { DaysRemainingToken, $"{DaysRemaining}" },
            };
        }

        public override string GetTemplateId(TemplateOptions options)
        {
            return options.ContractExpiryTemplateId;
        }
    }
}
