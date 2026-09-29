namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Competitions.PriceAndQuantity;

public enum CompetitionServiceType
{
    CatalogueSolution,
    AdditionalService
}

public static class CompetitionServiceTypeExtensions
{
    public static string ContainerId(this CompetitionServiceType type) => type switch
    {
        CompetitionServiceType.CatalogueSolution => "#SolutionDetails",
        CompetitionServiceType.AdditionalService => "#AdditionalServiceDetails",
        _ => "#SolutionDetails"
    };

    public static string PriceHeading(this CompetitionServiceType type) => type switch
    {
        CompetitionServiceType.CatalogueSolution => "Price of Catalogue solution",
        CompetitionServiceType.AdditionalService => "Price of Additional service",
        _ => "Price of Catalogue solution"
    };

    public static string QuantityHeading(this CompetitionServiceType type) => type switch
    {
        CompetitionServiceType.CatalogueSolution => "Quantity of catalogue solution",
        CompetitionServiceType.AdditionalService => "Quantity of additional service",
        _ => "Quantity of catalogue solution"
    };
}
