using System;
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
    public string BankfieldTotal = "2";
    public string BeechwoodTotal = "20";

    public string PriceOption = "Price 1: per patient";
    public string AwardCriteria = "Price only";
    public string PriceAndNonPriceCriteria = "Price and non-price elements";

    public string FeatureRequirementType = "Must";
    public string FeatureRequirement = TestDataGenerator.Sentence();
    public string ImplementationRequirement = TestDataGenerator.Sentence();

    public string[] InteroperabilityOptions = { "Bulk", "Access Record HTML", "Online Consultations" };

    public string ServiceLevelFrom = "09:00";
    public string ServiceLevelUntil = "18:00";

    public string PriceWeighting;
    public string NonPriceWeighting;
    public string FeaturesWeighting;
    public string ImplementationWeighting;
    public string InteroperabilityWeighting;
    public string ServiceLevelWeighting;

    // Non-price element scoring (per solution: index 0 and 1)
    public string FirstSolutionScore = TestDataGenerator.ScoreOneToFive();
    public string SecondSolutionScore = TestDataGenerator.ScoreOneToFive();
    public string FirstSolutionJustification = TestDataGenerator.Sentence();
    public string SecondSolutionJustification = TestDataGenerator.Sentence();

    public CompetitionTestData()
    {
        var price = TestDataGenerator.MultipleOfFive(30, 90);
        PriceWeighting = price.ToString();
        NonPriceWeighting = (100 - price).ToString();

        var nonPrice = TestDataGenerator.WeightingsSummingTo100(4);
        FeaturesWeighting = nonPrice[0].ToString();
        ImplementationWeighting = nonPrice[1].ToString();
        InteroperabilityWeighting = nonPrice[2].ToString();
        ServiceLevelWeighting = nonPrice[3].ToString();
    }
}
