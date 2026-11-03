namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ManageEpics;

public record Epic(string Name, string Reference, bool IsActive)
{
    public string Name { get; set; } = Name;

    public string Reference { get; set; } = Reference;

    public bool IsActive { get; set; } = IsActive;
}
