using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.NonPriceElementModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Models.NonPriceElementModels;

public static class NonPriceElementsModelTests
{
    [Theory]
    [MockAutoData]
    public static void Construct_SetsPropertiesAsExpected(
        Organisation organisation,
        NonPriceElements elements,
        Competition competition,
        List<Integration> integrations)
    {
        competition.Organisation = organisation;
        competition.NonPriceElements = elements;

        var model = new NonPriceElementsModel(competition, integrations);

        model.InternalOrgId.Should().Be(organisation.InternalIdentifier);
        model.CompetitionId.Should().Be(competition.Id);
        model.CompetitionName.Should().Be(competition.Name);
        model.NonPriceElements.Should().BeEquivalentTo(elements);
    }

    [Theory]
    [MockAutoData]
    public static void HasAllNonPriceElements_ReturnsExpected(
        Organisation organisation,
        Competition competition,
        List<Integration> integrations)
    {
        competition.Organisation = organisation;
        competition.NonPriceElements = new()
        {
            Implementation = new(),
            IntegrationTypes = new List<IntegrationType> { new() },
            Features = new List<FeaturesCriteria> { new() },
            ServiceLevel = new(),
        };

        var model = new NonPriceElementsModel(competition, integrations);

        model.HasAllNonPriceElements().Should().BeTrue();
    }

    [Theory]
    [MockAutoData]
    public static void HasAllNonPriceElements_None_ReturnsExpected(
        Organisation organisation,
        Competition competition,
        List<Integration> integrations)
    {
        competition.Organisation = organisation;
        competition.NonPriceElements = new();

        var model = new NonPriceElementsModel(competition, integrations);

        model.HasAllNonPriceElements().Should().BeFalse();
    }

    [Theory]
    [MockAutoData]
    public static void HasAnyNonPriceElements_ReturnsExpected(
        Organisation organisation,
        Competition competition,
        List<Integration> integrations)
    {
        competition.Organisation = organisation;
        competition.NonPriceElements = new()
        {
            Implementation = new(),
            IntegrationTypes = new List<IntegrationType> { new() },
            ServiceLevel = new(),
        };

        var model = new NonPriceElementsModel(competition, integrations);

        model.HasAnyNonPriceElements().Should().BeTrue();
    }

    [Theory]
    [MockAutoData]
    public static void HasAnyNonPriceElements_None_ReturnsExpected(
        Organisation organisation,
        Competition competition,
        List<Integration> integrations)
    {
        competition.Organisation = organisation;
        competition.NonPriceElements = new();

        var model = new NonPriceElementsModel(competition, integrations);

        model.HasAnyNonPriceElements().Should().BeFalse();
    }

    [Theory]
    [MockAutoData]
    public static void Advice_HasReviewedCriteria_ExpectedContent(
        Organisation organisation,
        Competition competition,
        List<Integration> integrations)
    {
        competition.HasReviewedCriteria = true;
        competition.Organisation = organisation;
        competition.NonPriceElements = new();

        var model = new NonPriceElementsModel(competition, integrations);

        model.Advice.Should()
            .Be("These are the non-price elements you added to help you score your shortlisted solutions.");
    }

    [Theory]
    [MockAutoData]
    public static void Advice_HasAllNonPriceElements_ExpectedContent(
        Organisation organisation,
        Competition competition,
        List<Integration> integrations)
    {
        competition.HasReviewedCriteria = false;
        competition.Organisation = organisation;
        competition.NonPriceElements = new()
        {
            Implementation = new(),
            IntegrationTypes = new List<IntegrationType> { new() },
            Features = new List<FeaturesCriteria> { new() },
            ServiceLevel = new(),
        };

        var model = new NonPriceElementsModel(competition, integrations);

        model.Advice.Should()
            .Be("All available non-price elements have been added for this competition.");
    }

    [Theory]
    [MockAutoData]
    public static void Advice_HasNoNonPriceElements_ExpectedContent(
        Organisation organisation,
        Competition competition,
        List<Integration> integrations)
    {
        competition.HasReviewedCriteria = false;
        competition.Organisation = organisation;
        competition.NonPriceElements = new();

        var model = new NonPriceElementsModel(competition, integrations);

        model.Advice.Should()
            .Be("Add at least 1 optional non-price element to help you score your shortlisted solutions, for example features, implementation, interoperability or service levels.");
    }
}
