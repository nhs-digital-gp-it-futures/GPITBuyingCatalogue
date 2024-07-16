using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.CapabilityModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models.CapabilityModels;

public static class CapabilityEpicModelTests
{
    [Theory]
    [MockAutoData]
    public static void Construct_SetsProperties_Active(
        Solution solution,
        Capability capability,
        Epic epic)
    {
        epic.IsActive = true;
        var model = new CapabilityEpicModel(solution.CatalogueItem, capability, epic);

        model.Id.Should().Be(epic.Id);
        model.Name.Should().Be(epic.Name);
        model.Selected.Should().BeFalse();
    }

    [Theory]
    [MockAutoData]
    public static void Construct_SetsProperties_Inactive(
        Solution solution,
        Capability capability,
        Epic epic)
    {
        epic.IsActive = false;
        var model = new CapabilityEpicModel(solution.CatalogueItem, capability, epic);

        model.Id.Should().Be(epic.Id);
        model.Name.Should().Be(epic.NameWithStatusSuffix);
        model.Selected.Should().BeFalse();
    }

    [Theory]
    [MockAutoData]
    public static void Construct_SelectedEpics_SetsSelected(
        Solution solution,
        Capability capability,
        Epic epic)
    {
        epic.Capabilities = new List<Capability> { capability };

        solution.CatalogueItem.CatalogueItemEpics.Add(
            new(solution.CatalogueItemId, capability.Id, epic.Id));

        var model = new CapabilityEpicModel(solution.CatalogueItem, capability, epic);

        model.Selected.Should().BeTrue();
    }
}
