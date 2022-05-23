using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ListPriceModels
{
    public class EditTieredListPriceModel : AddTieredListPriceModel
    {
        public EditTieredListPriceModel()
        {
        }

        public EditTieredListPriceModel(CatalogueItem catalogueItem, CataloguePrice price, int maximumNumberOfTiers)
            : base(catalogueItem, price)
        {
            Tiers = price.CataloguePriceTiers.ToList();
            SelectedPublicationStatus
                = CataloguePricePublicationStatus
                = price.PublishedStatus;

            MaximumNumberOfTiers = maximumNumberOfTiers;
        }

        public PublicationStatus CataloguePricePublicationStatus { get; set; }

        public PublicationStatus SelectedPublicationStatus { get; set; }

        public IList<SelectableRadioOption<PublicationStatus>> AvailablePublicationStatuses => CataloguePricePublicationStatus
            .GetAvailablePublicationStatuses()
            .Select(p => new SelectableRadioOption<PublicationStatus>(p.Description(), p))
            .ToList();

        public IList<CataloguePriceTier> Tiers { get; set; }

        public int MaximumNumberOfTiers { get; set; }
    }
}
