using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.TestData.Generators;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.TestData;

public class CompetitionTestData
{
    public string BaseUrl { get; set; } = "https://localhost:5001";

    public string Email { get; set; } = "suesmith@email.com";
    public string Password { get; set; } = "Pass123$";

    public string CompetitionName = $"Test Competition {DateTime.UtcNow:HHmmssfff}";
    public string CompetitionDescription = TestDataGenerator.Sentence();
    public string ExclusionReason = TestDataGenerator.Sentence();

    public string ShortlistValue = "1";
    public string FirstSolution = "Emis Web GP";
    public string SecondSolution = "Write on Time";
    public string Sublocation = "02T";
    public string ContractLength = "18";
    public string[] Practices = { "BANKFIELD SURGERY", "BEECHWOOD MEDICAL CENTRE" };
    public string AwardCriteria = "Price only";

    // Patient totals per practice, used across all price/quantity blocks
    public string BankfieldTotal = "2";
    public string BeechwoodTotal = "20";

    // Price option for solutions that require one (e.g. Write on Time)
    public string PriceOption = "Price 1: per patient";
}
