using System.Collections.Generic;
using System.Linq;
using EnumsNET;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models
{
    public sealed class CatalogueSolutionsModel
    {
        private readonly List<PublicationStatus> publicationStatusModels =
            Enums.GetValues<EntityFramework.Models.GPITBuyingCatalogue.PublicationStatus>(
                    EnumMemberSelection.DisplayOrder)
                .Select(s => new PublicationStatus { Id = (int)s, Display = s.AsString(EnumFormat.DisplayName) })
                .ToList();

        public CatalogueSolutionsModel(IEnumerable<CatalogueItem> solutions)
        {
            Solutions = solutions?.Select(s => new CatalogueModel(s)).ToList();
        }

        public IList<PublicationStatus> AllPublicationStatuses => publicationStatusModels;

        public List<CatalogueModel> Solutions { get; set; }

        public bool HasSelected => publicationStatusModels.Any(p => p.Checked);

        public void SetSelected(EntityFramework.Models.GPITBuyingCatalogue.PublicationStatus publicationStatus)
        {
            if (publicationStatusModels.SingleOrDefault(p => p.Id == (int)publicationStatus) is
                { } publicationStatusModel)
                publicationStatusModel.Checked = true;
        }
    }
}
