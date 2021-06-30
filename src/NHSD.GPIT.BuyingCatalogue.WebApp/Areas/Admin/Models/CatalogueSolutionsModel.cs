using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models
{
    public sealed class CatalogueSolutionsModel
    {
        private readonly List<PublicationStatusModel> publicationStatusModels = Enum.GetValues<PublicationStatus>()
            .ToList()
            .Select(p => new PublicationStatusModel { Id = (int)p, Display = p.GetDisplayName(), })
            .ToList();

        public CatalogueSolutionsModel(IList<CatalogueItem> solutions)
        {
            Solutions = solutions.Select(s => new CatalogueModel(s)).ToList();
        }

        public IList<PublicationStatusModel> AllPublicationStatuses => publicationStatusModels;

        public List<CatalogueModel> Solutions { get; set; }

        public bool HasSelected => publicationStatusModels.Any(p => p.Checked);

        public void SetSelected(PublicationStatus publicationStatus)
        {
            if (publicationStatusModels.SingleOrDefault(p => p.Id == (int)publicationStatus) is
                { } publicationStatusModel)
                publicationStatusModel.Checked = true;
        }
    }
}
