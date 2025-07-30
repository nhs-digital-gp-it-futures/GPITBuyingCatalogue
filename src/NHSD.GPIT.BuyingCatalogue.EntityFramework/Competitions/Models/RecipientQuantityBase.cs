namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

public abstract class RecipientQuantityBase
{
    public string ParentSublocationOdsCode { get; set; }

    public string RecipientOdsCode { get; set; }

    public int Quantity { get; set; }
}
