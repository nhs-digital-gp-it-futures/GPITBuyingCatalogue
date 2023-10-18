using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Competitions;

public static class CompetitionsDashboardObjects
{
    public static By CompetitionsTable => ByExtensions.DataTestId("competitions-table");

    public static By CreateCompetitionLink => By.LinkText("Create a new competition");

    public static By SelectAllServiceRecipients => By.LinkText("Select all");

    public static By CreateManageCompetitionLink => By.LinkText("Create or manage competitions");

    public static By ServiceRecipientsLink => By.LinkText("Service Recipients");

    public static By ContractLengthLink => By.LinkText("Contract length");

    public static By ContractLengthInput => By.Id("ContractLength");

    public static By AwardCriteriaLink => By.LinkText("Award criteria");
}
