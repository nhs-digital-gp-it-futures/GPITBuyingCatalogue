using System;
using System.Collections.Generic;
using System.Linq;
using EnumsNET;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.BaseModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ClientApplicationTypeModels
{
    public class ClientApplicationTypeSectionModel : MarketingBaseModel
    {
        public ClientApplicationTypeSectionModel()
            : base(null)
        {
        }

        public ClientApplicationTypeSectionModel(CatalogueItem catalogueItem)
            : base(catalogueItem)
        {
            if (catalogueItem is null)
                throw new ArgumentNullException(nameof(catalogueItem));

            var applicationType = catalogueItem.Solution?.GetClientApplication();

            SolutionName = catalogueItem.Name;
            ExistingClientApplicationTypes = applicationType!.ExistingClientApplicationTypes ?? Array.Empty<ApplicationType>();
            ApplicationTypesToAdd = Enum.GetValues<ApplicationType>().Except(ExistingClientApplicationTypes).ToList();
            ApplicationTypesToAddRadioItems = ApplicationTypesToAdd.Select(t => new { Text = t.AsString(EnumFormat.DisplayName), Value = t.ToString() });
            ExistingApplicationTypesCount = ExistingClientApplicationTypes.Count;
        }

        public IReadOnlyList<ApplicationType> ExistingClientApplicationTypes { get; } = Array.Empty<ApplicationType>();

        public int? ExistingApplicationTypesCount { get; set; }

        public string SolutionName { get; set; }

        public List<ApplicationType> ApplicationTypesToAdd { get; set; }

        public IEnumerable<object> ApplicationTypesToAddRadioItems { get; }
    }
}
