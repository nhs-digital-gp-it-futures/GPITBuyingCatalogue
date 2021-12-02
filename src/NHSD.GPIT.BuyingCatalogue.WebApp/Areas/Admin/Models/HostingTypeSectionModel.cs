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
    public class HostingTypeSectionModel : MarketingBaseModel
    {
        public HostingTypeSectionModel()
            : base(null)
        {
        }

        public HostingTypeSectionModel(CatalogueItem catalogueItem)
            : base(catalogueItem)
        {
            if (catalogueItem is null)
                throw new ArgumentNullException(nameof(catalogueItem));

            Hosting = catalogueItem.Solution?.Hosting ?? new Hosting();

            SolutionName = catalogueItem.Name;
            ExistingHostingTypes = Hosting?.AvailableHosting() ?? Array.Empty<HostingType>();
            HostingTypesToAdd = Enum.GetValues<HostingType>().Except(ExistingHostingTypes).ToList();
            HostingTypesToAddRadioItems = HostingTypesToAdd.Select(t => new { Text = t.AsString(EnumFormat.DisplayName), Value = t.ToString() });
            ExistingHostingTypesCount = ExistingHostingTypes.Count;
        }

        public Hosting Hosting { get; set; }

        public IReadOnlyList<HostingType> ExistingHostingTypes { get; } = Array.Empty<HostingType>();

        public int? ExistingHostingTypesCount { get; set; }

        public string SolutionName { get; set; }

        public List<HostingType> HostingTypesToAdd { get; set; }

        public IEnumerable<object> HostingTypesToAddRadioItems { get; }
    }
}
