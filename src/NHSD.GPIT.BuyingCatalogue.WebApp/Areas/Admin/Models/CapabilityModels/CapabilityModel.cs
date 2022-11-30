﻿using System.Collections.Generic;
using System.Linq;
using MoreLinq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.CapabilityModels
{
    public sealed class CapabilityModel
    {
        public CapabilityModel()
        {
        }

        public CapabilityModel(
            CatalogueItem catalogueItem,
            Capability capability)
        {
            Id = capability.Id;
            Name = capability.Name;
            CapabilityRef = capability.CapabilityRef;
            Selected = catalogueItem.CatalogueItemCapabilities.Any(
                itemCapability => itemCapability.CapabilityId == capability.Id);
            MayEpics = GetEpics(catalogueItem, capability, CompliancyLevel.May);
            MustEpics = GetEpics(catalogueItem, capability, CompliancyLevel.Must);

            if (!Selected)
                MustEpics.ForEach(e => e.Selected = true);
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string CapabilityRef { get; set; }

        public bool Selected { get; set; }

        public IList<CapabilityEpicModel> MustEpics { get; set; } = new List<CapabilityEpicModel>();

        public IList<CapabilityEpicModel> MayEpics { get; set; } = new List<CapabilityEpicModel>();

        public IEnumerable<CapabilityEpicModel> Epics => MustEpics.Concat(MayEpics).ToList();

        private static IList<CapabilityEpicModel> GetEpics(CatalogueItem catalogueItem, Capability capability, CompliancyLevel level)
        {
            return capability.Epics.Where(e => e.CompliancyLevel == level)
                .Select(
                    epic => new CapabilityEpicModel(catalogueItem, capability, epic))
                .OrderBy(e => e.Id)
                .ToList();
        }
    }
}
