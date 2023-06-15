using System;
using System.Collections.Generic;
using System.Linq;
using EnumsNET;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.BaseModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ApplicationTypeModels
{
    public class ApplicationTypeSectionModel : MarketingBaseModel
    {
        public ApplicationTypeSectionModel()
            : base(null)
        {
        }

        public ApplicationTypeSectionModel(CatalogueItem catalogueItem)
            : base(catalogueItem)
        {
            if (catalogueItem is null)
                throw new ArgumentNullException(nameof(catalogueItem));

            var applicationType = catalogueItem.Solution?.GetApplicationTypes();

            SolutionName = catalogueItem.Name;
            ExistingApplicationTypes = applicationType!.ExistingApplicationTypes ?? Array.Empty<ApplicationType>();
            ApplicationTypesToAdd = Enum.GetValues<ApplicationType>().Except(ExistingApplicationTypes).ToList();
            ApplicationTypesToAddRadioItems = ApplicationTypesToAdd.Select(t => new { Text = t.AsString(EnumFormat.DisplayName), Value = t.ToString() });
            ExistingApplicationTypesCount = ExistingApplicationTypes.Count;
        }

        public IReadOnlyList<ApplicationType> ExistingApplicationTypes { get; } = Array.Empty<ApplicationType>();

        public int? ExistingApplicationTypesCount { get; set; }

        public string SolutionName { get; set; }

        public List<ApplicationType> ApplicationTypesToAdd { get; set; }

        public IEnumerable<object> ApplicationTypesToAddRadioItems { get; }
    }
}
