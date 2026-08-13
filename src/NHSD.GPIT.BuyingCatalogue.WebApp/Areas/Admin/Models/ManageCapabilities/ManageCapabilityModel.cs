using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.AdminManageCapability;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.AdminManageEpic;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ManageCapabilities;

public class ManageCapabilityModel : NavBaseModel
{
    public ManageCapabilityModel()
    {
    }

    public ManageCapabilityModel(Capability capability)
    {
        Id = capability.Id;
        CapabilityRef = capability.CapabilityRef;
        Name = capability.Name;
        Description = capability.Description;
        SourceUrl = capability.SourceUrl;
        Status = capability.Status;
        EpicsExpanderModel = new CapabilityEpicsExpanderModel
        {
            Epics = capability.Epics.Select(x => new AdminManageEpic
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                IsActive = x.IsActive,
            }),
        };
        CatalogueItemsExpanderModel = new CapabilityCatalogueItemsExpanderModel
        {
            CatalogueItems = capability.CatalogueItemCapabilities.Select(x => new AdminManageCapabilityCatalogueItem
            {
                Id = x.CatalogueItem.Id,
                Name = x.CatalogueItem.Name,
                SupplierName = x.CatalogueItem.Supplier.Name,
                CatalogueItemType = x.CatalogueItem.CatalogueItemType,
                PublishedStatus = x.CatalogueItem.PublishedStatus,
            }),
        };
    }

    public int Id { get; set; }

    public string CapabilityRef { get; set; }

    public string Name { get; set; }

    [StringLength(500)]
    public string Description { get; set; }

    public string SourceUrl { get; set; }

    public CapabilityStatus Status { get; set; }

    public CapabilityEpicsExpanderModel EpicsExpanderModel { get; set; }

    public CapabilityCatalogueItemsExpanderModel CatalogueItemsExpanderModel { get; set; }

    public IReadOnlyList<SelectOption<CapabilityStatus>> AvailableStatuses =>
        [
            new SelectOption<CapabilityStatus>("Effective", CapabilityStatus.Effective),
            new SelectOption<CapabilityStatus>("Expired", CapabilityStatus.Expired)
        ];
}
