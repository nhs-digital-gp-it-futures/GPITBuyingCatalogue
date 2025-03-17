namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Models;

public record CheckboxNameAndValueModel
{
    public const bool Selected = true;
    public const bool NotSelected = false;

    public string Name { get; init; }

    public bool Value { get; init; }
}
