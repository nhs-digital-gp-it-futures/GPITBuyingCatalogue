using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Notifications.Models;

namespace BuyingCatalogueFunction.Notifications.Templates
{
    public class ContractDueToExpireEmailTemplate(ContractDueToExpireEmailModel model) : GovNotifyEmailTemplate(EmailNotificationTypeEnum.ContractDueToExpire)
    {
        public const string OrderIdToken = "order_id";
        public const string FirstNameToken = "first_name";
        public const string LastNameToken = "last_name";
        public const string DaysRemainingToken = "number_of_days";
        public ContractDueToExpireEmailModel Model { get; set; } = model;

        public override Dictionary<string, dynamic> GetTemplatePersonalisation()
        {
            return new Dictionary<string, dynamic>
            {
                { OrderIdToken, $"{Model.CallOffId}" },
                { FirstNameToken, $"{Model.FirstName}" },
                { LastNameToken, $"{Model.LastName}" },
                { DaysRemainingToken, $"{Model.DaysRemaining}" },
            };
        }

        public override string GetTemplateId(TemplateOptions options)
        {
            return options.ContractExpiryTemplateId;
        }
    }
}
