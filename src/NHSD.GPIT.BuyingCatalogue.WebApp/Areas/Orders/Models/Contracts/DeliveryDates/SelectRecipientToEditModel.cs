namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.DeliveryDates;

public class SelectRecipientToEditModel
{
    public SelectRecipientToEditModel()
    {
    }

    public bool Selected { get; set; }

    public required string ParentSublocationName { get; set; }

    public required string ParentSublocationOdsCode { get; set; }

    public required string RecipientOrganisationId { get; set; }

    public required string RecipientName { get; set; }

    public required string RecipientOdsCode { get; set; }

    public string Status { get; set; }
}
