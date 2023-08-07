using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

public class CompetitionRecipient
{
    public CompetitionRecipient()
    {
    }

    public CompetitionRecipient(
        int competitionId,
        string odsCode)
    {
        CompetitionId = competitionId;
        OdsCode = odsCode;
    }

    public int CompetitionId { get; set; }

    public string OdsCode { get; set; }

    public Competition Competition { get; set; }

    public OdsOrganisation OdsOrganisation { get; set; }

    public ICollection<RecipientQuantity> Quantities { get; set; } = new HashSet<RecipientQuantity>();

    public void SetQuantityForItem(CatalogueItemId catalogueItemId, int quantity)
    {
        var recipientQuantity = GetRecipientQuantityForItem(catalogueItemId);

        if (recipientQuantity == null)
        {
            recipientQuantity = new RecipientQuantity();
            Quantities.Add(recipientQuantity);
        }

        recipientQuantity.Quantity = quantity;
    }

    public RecipientQuantity GetRecipientQuantityForItem(CatalogueItemId catalogueItemId) =>
        Quantities.FirstOrDefault(x => x.CatalogueItemId == catalogueItemId);
}
