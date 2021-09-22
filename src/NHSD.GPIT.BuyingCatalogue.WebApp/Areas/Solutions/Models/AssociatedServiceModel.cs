using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public sealed class AssociatedServiceModel
    {
        public AssociatedServiceModel(AssociatedService associatedService)
        {
            Description = associatedService.Description;
            Name = associatedService.CatalogueItem.Name;
            OrderGuidance = associatedService.OrderGuidance;
            Prices = associatedService.CatalogueItem.CataloguePrices
                .Select(p => $"£{p.Price.GetValueOrDefault():F} {p.PricingUnit.Description}").ToList();
        }

        public string Description { get; }

        public string Name { get; }

        public string OrderGuidance { get; }

        public IList<string> Prices { get; }
    }
}
