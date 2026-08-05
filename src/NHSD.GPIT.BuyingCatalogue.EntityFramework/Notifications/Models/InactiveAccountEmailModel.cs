using System.Collections.Generic;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Notifications.Models;

public class InactiveAccountEmailModel : GovNotifyEmailModel
{
    public const string DeactivationCountdownToken = "deactivation_countdown";
    public const string DayStyleToken = "day_style";

    private const string DaySingular = "day";
    private const string DayPlural = "days";

    public InactiveAccountEmailModel()
        : base(EmailNotificationTypeEnum.InactiveAccount)
    {
    }

    public int DaysFromThreshold { get; set; }

    public override Dictionary<string, dynamic> GetTemplatePersonalisation() =>
        new()
        {
            { DeactivationCountdownToken, DaysFromThreshold },
            { DayStyleToken, DaysFromThreshold == 1 ? DaySingular : DayPlural },
        };

    public override string GetTemplateId(TemplateOptions options) => options.InactiveAccountTemplateId;
}
