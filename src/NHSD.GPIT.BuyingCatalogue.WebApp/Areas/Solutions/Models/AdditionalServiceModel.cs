using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public sealed class AdditionalServiceModel
    {
        public AdditionalServiceModel(CatalogueItem additionalService)
        {
            if (additionalService is null)
                throw new ArgumentNullException(nameof(additionalService));

            SolutionId = additionalService.Id;
            Description = additionalService.AdditionalService.FullDescription;
            Name = additionalService.Name;
            Prices = additionalService.CataloguePrices.Select(cp => cp.ToString()).ToList();
        }

        public CatalogueItemId SolutionId { get; set; }

        public string Description { get; set; }

        public string Name { get; set; }

        public IList<string> Prices { get; set; }
    }
}
