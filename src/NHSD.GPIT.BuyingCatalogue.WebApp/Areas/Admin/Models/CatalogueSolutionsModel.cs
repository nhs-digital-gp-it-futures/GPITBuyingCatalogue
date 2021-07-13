using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models
{
    public sealed class CatalogueSolutionsModel
    {
        public CatalogueSolutionsModel()
        {
        }

        public CatalogueSolutionsModel(IEnumerable<CatalogueItem> solutions)
        {
            SetSolutions(solutions);
        }

        public IList<PublicationStatus> PublicationStatuses { get; } = Enum.GetValues<PublicationStatus>().ToList().AsReadOnly();

        public IList<CatalogueModel> Solutions { get; } = new List<CatalogueModel>();

        public string SelectedPublicationStatus { get; set; }

        public void SetSolutions(IEnumerable<CatalogueItem> solutions)
        {
            if (solutions is null)
                return;

            Solutions.Clear();

            foreach (var item in solutions.Select(s => new CatalogueModel(s)).ToList())
                Solutions.Add(item);
        }
    }
}
