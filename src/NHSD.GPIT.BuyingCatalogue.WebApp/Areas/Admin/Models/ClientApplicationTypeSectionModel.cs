using System;
using System.Collections.Generic;
using System.Linq;
using EnumsNET;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;

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

        public TaskProgress ClientApplicationTypeStatus()
        {
            if (!ExistingClientApplicationTypes.Any())
                return TaskProgress.NotStarted;

            var statuses = ExistingClientApplicationTypes.Select(c => ClientApplication.ApplicationTypeStatus(c));

            if (statuses.All(s => s == TaskProgress.Completed))
                return TaskProgress.Completed;

            if (statuses.Any(s => s == TaskProgress.InProgress))
                return TaskProgress.InProgress;

            return TaskProgress.NotStarted;
        }
    }
}
