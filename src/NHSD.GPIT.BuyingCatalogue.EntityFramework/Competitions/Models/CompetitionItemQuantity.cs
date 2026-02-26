namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

public class CompetitionItemQuantity
{
    public int Id { get; set; }

    public int CompetitionId { get; set; }

    public string ParentSublocationOdsCode { get; set; }

    public string RecipientOdsCode { get; set; }

    public int CompetitionItemId { get; set; }

    public int? Quantity { get; set; }

    public CompetitionSublocationRecipient Recipient { get; set; }
}
