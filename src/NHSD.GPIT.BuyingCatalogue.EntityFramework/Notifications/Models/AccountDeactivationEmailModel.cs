using System.Collections.Generic;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Notifications.Models;

public class AccountDeactivationEmailModel : GovNotifyEmailModel
{
    public AccountDeactivationEmailModel()
        : base(EmailNotificationTypeEnum.AccountDeactivation)
    {
    }

    public override Dictionary<string, dynamic> GetTemplatePersonalisation() => [];

    public override string GetTemplateId(TemplateOptions options) => options.AccountDeactivationTemplateId;
}
