using System;
using System.Collections.Generic;
using System.Linq;
using EnumsNET;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models
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
            ExistingClientApplicationTypes = applicationType!.ExistingClientApplicationTypes ?? Array.Empty<ClientApplicationType>();
            ApplicationTypesToAdd = Enum.GetValues<ClientApplicationType>().Except(ExistingClientApplicationTypes).ToList();
            ApplicationTypesToAddRadioItems = ApplicationTypesToAdd.Select(t => new { Text = t.AsString(EnumFormat.DisplayName), Value = t.ToString() });
            ExistingApplicationTypesCount = ExistingClientApplicationTypes.Count;
            BackLink = $"/admin/catalogue-solutions/manage/{catalogueItem.Id}";
        }

        public IReadOnlyList<ClientApplicationType> ExistingClientApplicationTypes { get; } = Array.Empty<ClientApplicationType>();

        public int? ExistingApplicationTypesCount { get; set; }

        public string SolutionName { get; set; }

        public List<ClientApplicationType> ApplicationTypesToAdd { get; set; }

        public IEnumerable<object> ApplicationTypesToAddRadioItems { get; }

        // mjrxxxx
        //public override bool? IsComplete => ExistingClientApplicationTypes.Any();

        // mjrtodo - this needs to be InProgress if any of the client application types are in progress
        public FeatureCompletionStatus StatusClientApplicationType() => ExistingClientApplicationTypes.Any()
            ? FeatureCompletionStatus.Completed
            : FeatureCompletionStatus.NotStarted;
    }
}
