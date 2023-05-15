using System;
using System.Collections.Generic;
using System.Linq;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared;

public class ExpiredFrameworksModel
{
    internal const string SingleExpiredFramework =
        "This solution is currently only available from a framework which has expired. It cannot be purchased using the Buying Catalogue at this time.";

    internal const string MultipleExpiredFrameworks =
        "This solution is currently only available from frameworks which have expired. It cannot be purchased using the Buying Catalogue at this time.";

    internal const string SingleExpiredWithActiveFrameworks =
        "This solution is available from more than 1 framework. The {0} framework has expired, so this solution can no longer be purchased under that framework.";

    internal const string MultipleExpiredWithActiveFrameworks =
        "This solution is available from more than 1 framework. The {0} framework have expired, so this solution can no longer be purchased under those frameworks.";

    public ExpiredFrameworksModel(IList<EntityFramework.Catalogue.Models.Framework> frameworks)
    {
        Frameworks = frameworks;
    }

    public IList<EntityFramework.Catalogue.Models.Framework> Frameworks { get; set; }

    public string GetWarningText()
    {
        var expiredFrameworks = Frameworks.Count(x => x.IsExpired);
        var activeFrameworks = Frameworks.Count(x => !x.IsExpired);

        var warningText = expiredFrameworks switch
        {
            1 when activeFrameworks == 0 =>
                SingleExpiredFramework,
            > 1 when activeFrameworks == 0 =>
                MultipleExpiredFrameworks,
            1 when activeFrameworks > 0 =>
                string.Format(SingleExpiredWithActiveFrameworks, Frameworks.First(x => x.IsExpired).ShortName),
            > 1 when activeFrameworks > 0 =>
                string.Format(
                    MultipleExpiredWithActiveFrameworks,
                    string.Join(", ", Frameworks.Where(x => x.IsExpired).Select(x => x.ShortName))),
            _ => throw new ArgumentOutOfRangeException(),
        };

        return warningText;
    }
}
