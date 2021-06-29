using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models
{
    public sealed class CatalogueSolutionsModel
    {
        public IList<PublicationStatusModel> AllPublicationStatuses => Enum.GetValues<PublicationStatus>()
            .ToList()
            .Select(p => new PublicationStatusModel
            {
                Id = (int)p,
                Display = p.GetDisplayName(),
            })
            .ToList();

        public List<CatalogueModel> Solutions { get; set; }

        public int PublicationStatusId { get; set; }
    }
}
