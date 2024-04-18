using System.Collections.Generic;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Notifications.Models;

public class PasswordDueToExpireEmailModel : GovNotifyEmailModel
{
    public const string FirstNameToken = "first_name";
    public const string LastNameToken = "last_name";
    public const string DaysRemainingToken = "number_of_days";
    public const string DayStyleToken = "day_style";

    private const string DaySingular = "day";
    private const string DayPlural = "days";

    public PasswordDueToExpireEmailModel()
        : base(EmailNotificationTypeEnum.PasswordDueToExpire)
    {
    }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public int DaysRemaining { get; set; }

    public override Dictionary<string, dynamic> GetTemplatePersonalisation()
    {
        return new Dictionary<string, dynamic>
        {
            { FirstNameToken, FirstName },
            { LastNameToken, LastName },
            { DaysRemainingToken, DaysRemaining },
            { DayStyleToken, DaysRemaining == 1 ? DaySingular : DayPlural },
        };
    }

    public override string GetTemplateId(TemplateOptions options) => options.PasswordExpiryTemplateId;
}
