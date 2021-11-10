using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ServiceLevelAgreements
{
    public sealed class AddSlaTypeModel : NavBaseModel
    {
        public AddSlaTypeModel()
        {
        }

        public AddSlaTypeModel(Solution solution)
            : this()
        {
            SolutionId = solution.CatalogueItemId;

            if (solution.ServiceLevelAgreement is not null)
                SlaLevel = solution.ServiceLevelAgreement.SlaType;
        }

        public AddSlaTypeModel(CatalogueItem catalogueItem)
            : this(catalogueItem.Solution)
        {
            SolutionName = catalogueItem.Name;
        }

        public CatalogueItemId SolutionId { get; init; }

        public string Title { get; init; }

        public SlaType? SlaLevel { get; set; }

        public string SolutionName { get; }

        public IList<SlaType> SlaTypes { get; } = Enum.GetValues<SlaType>().ToList();
    }
}
