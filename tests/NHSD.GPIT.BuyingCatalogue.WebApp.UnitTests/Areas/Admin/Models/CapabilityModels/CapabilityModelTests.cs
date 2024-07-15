using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.CapabilityModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models.CapabilityModels;

public static class CapabilityModelTests
{
    [Theory]
    [MockAutoData]
    public static void Construct_SetsProperties_Effective(
        Solution solution,
        Capability capability)
    {
        capability.Status = CapabilityStatus.Effective;

        var model = new CapabilityModel(solution.CatalogueItem, capability);

        model.Id.Should().Be(capability.Id);
        model.Name.Should().Be(capability.Name);
        model.CapabilityRef.Should().Be(capability.CapabilityRef);
        model.Selected.Should().BeFalse();
    }

    [Theory]
    [MockAutoData]
    public static void Construct_SetsProperties_Expired(
        Solution solution,
        Capability capability)
    {
        capability.Status = CapabilityStatus.Expired;

        var model = new CapabilityModel(solution.CatalogueItem, capability);

        model.Id.Should().Be(capability.Id);
        model.Name.Should().Be(capability.NameWithStatusSuffix);
        model.CapabilityRef.Should().Be(capability.CapabilityRef);
        model.Selected.Should().BeFalse();
    }

    [Theory]
    [MockAutoData]
    public static void Construct_Selected_SetsSelected(
        Solution solution,
        Capability capability)
    {
        solution.CatalogueItem.CatalogueItemCapabilities.Add(new(solution.CatalogueItemId, capability.Id));

        var model = new CapabilityModel(solution.CatalogueItem, capability);

        model.Selected.Should().BeTrue();
    }

    [Theory]
    [MockAutoData]
    public static void Construct_Epics_SplitsMustAndMay(
        Solution solution,
        Capability capability,
        List<Epic> mustEpics,
        List<Epic> mayEpics)
    {
        mayEpics.ForEach(e => e.IsActive = true);
        mustEpics.ForEach(e => e.IsActive = true);

        var mustCapabilityEpics = mustEpics
            .Select(e => new CapabilityEpic() { Epic = e, CompliancyLevel = CompliancyLevel.Must });
        var mayCapabilityEpics = mayEpics
            .Select(e => new CapabilityEpic() { Epic = e, CompliancyLevel = CompliancyLevel.May });

        capability.CapabilityEpics = mustCapabilityEpics.Concat(mayCapabilityEpics).ToList();
        solution.CatalogueItem.CatalogueItemCapabilities.Add(new(solution.CatalogueItemId, capability.Id));

        var model = new CapabilityModel(solution.CatalogueItem, capability);

        model.MustEpics.Should().HaveCount(mustEpics.Count);
        model.MayEpics.Should().HaveCount(mayEpics.Count);
    }

    [Theory]
    [MockAutoData]
    public static void Construct_Effective_CapabilityNotSelected_SelectsAllMustEpics(
        Solution solution,
        Capability capability,
        List<Epic> mustEpics,
        List<Epic> mayEpics)
    {
        capability.Status = CapabilityStatus.Effective;

        mayEpics.ForEach(e => e.IsActive = true);
        mustEpics.ForEach(e => e.IsActive = true);

        var mustCapabilityEpics = mustEpics
            .Select(e => new CapabilityEpic() { Epic = e, CompliancyLevel = CompliancyLevel.Must });
        var mayCapabilityEpics = mayEpics
            .Select(e => new CapabilityEpic() { Epic = e, CompliancyLevel = CompliancyLevel.May });

        capability.CapabilityEpics = mustCapabilityEpics.Concat(mayCapabilityEpics).ToList();
        solution.CatalogueItem.CatalogueItemCapabilities = Enumerable.Empty<CatalogueItemCapability>().ToList();

        var model = new CapabilityModel(solution.CatalogueItem, capability);

        model.MustEpics.Should().OnlyContain(e => e.Selected == true);
        model.MustEpics.Should().HaveCount(mustEpics.Count);
        model.MayEpics.Should().HaveCount(mayEpics.Count);
    }

    [Theory]
    [MockAutoData]
    public static void Construct_CapabilitySelected_PreservesMustEpics(
        Solution solution,
        Capability capability,
        List<Epic> mustEpics,
        List<Epic> mayEpics)
    {
        mayEpics.ForEach(e => e.IsActive = true);
        mustEpics.ForEach(e => e.IsActive = true);

        var mustCapabilityEpics = mustEpics
            .Select(e => new CapabilityEpic() { Epic = e, CompliancyLevel = CompliancyLevel.Must });
        var mayCapabilityEpics = mayEpics
            .Select(e => new CapabilityEpic() { Epic = e, CompliancyLevel = CompliancyLevel.May });

        capability.CapabilityEpics = mustCapabilityEpics.Concat(mayCapabilityEpics).ToList();
        solution.CatalogueItem.CatalogueItemCapabilities.Add(new(solution.CatalogueItemId, capability.Id));
        solution.CatalogueItem.CatalogueItemEpics = new List<CatalogueItemEpic>
        {
            new(solution.CatalogueItemId, capability.Id, mustEpics.First().Id),
        };

        var model = new CapabilityModel(solution.CatalogueItem, capability);

        model.MustEpics.Should().ContainSingle(e => e.Selected == true);
        model.MustEpics.Should().HaveCount(mustEpics.Count);
        model.MayEpics.Should().HaveCount(mayEpics.Count);
    }
}
