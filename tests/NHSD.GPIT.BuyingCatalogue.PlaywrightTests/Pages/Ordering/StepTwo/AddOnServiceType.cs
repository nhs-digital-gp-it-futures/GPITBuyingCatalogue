namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Ordering.StepTwo;

public enum AddOnServiceType
{
    Associated,
    Additional
}

public static class AddOnServiceTypeExtensions
{
    public static string LinkText(this AddOnServiceType type) => type switch
    {
        AddOnServiceType.Associated => "Add Associated services",
        AddOnServiceType.Additional => "Add Additional services",
        _ => throw new System.ArgumentOutOfRangeException(nameof(type))
    };

    public static string PriceHeading(this AddOnServiceType type) => type switch
    {
        AddOnServiceType.Associated => "Price of Associated service",
        AddOnServiceType.Additional => "Price of Additional service",
        _ => throw new System.ArgumentOutOfRangeException(nameof(type))
    };

    public static string QuantityHeading(this AddOnServiceType type) => type switch
    {
        AddOnServiceType.Associated => "Quantity of associated service",
        AddOnServiceType.Additional => "Quantity of additional service",
        _ => throw new System.ArgumentOutOfRangeException(nameof(type))
    };
}
