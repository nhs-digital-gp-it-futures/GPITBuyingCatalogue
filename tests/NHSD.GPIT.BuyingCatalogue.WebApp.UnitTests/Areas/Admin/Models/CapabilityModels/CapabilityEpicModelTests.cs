﻿using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.CapabilityModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models.CapabilityModels;

public static class CapabilityEpicModelTests
{
    [Theory]
    [CommonAutoData]
    public static void Construct_SetsProperties(
        Solution solution,
        Capability capability,
        Epic epic)
    {
        var model = new CapabilityEpicModel(solution.CatalogueItem, capability, epic);

        model.Id.Should().Be(epic.Id);
        model.Name.Should().Be(epic.Name);
        model.Selected.Should().BeFalse();
    }

    [Theory]
    [CommonAutoData]
    public static void Construct_SelectedEpics_SetsSelected(
        Solution solution,
        Capability capability,
        Epic epic)
    {
        epic.CapabilityId = capability.Id;
        epic.Capability = capability;

        solution.CatalogueItem.CatalogueItemEpics.Add(
            new(solution.CatalogueItemId, capability.Id, epic.Id));

        var model = new CapabilityEpicModel(solution.CatalogueItem, capability, epic);

        model.Selected.Should().BeTrue();
    }
}
