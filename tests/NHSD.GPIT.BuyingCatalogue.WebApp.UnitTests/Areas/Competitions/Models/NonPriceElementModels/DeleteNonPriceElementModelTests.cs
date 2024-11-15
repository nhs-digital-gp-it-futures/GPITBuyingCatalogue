using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.NonPriceElementModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Models.NonPriceElementModels;

public static class DeleteNonPriceElementModelTests
{
    [Theory]
    [MockInlineAutoData(NonPriceElement.Implementation)]
    [MockInlineAutoData(NonPriceElement.Interoperability)]
    [MockInlineAutoData(NonPriceElement.ServiceLevel)]
    public static void Construct_SetsPropertiesAsExpected_NotFeatures(
        NonPriceElement nonPriceElementType,
        Organisation organisation,
        Competition competition)
    {
        competition.Organisation = organisation;

        var model = new DeleteNonPriceElementModel(nonPriceElementType, competition);

        model.InternalOrgId.Should().Be(organisation.InternalIdentifier);
        model.CompetitionId.Should().Be(competition.Id);
        model.CompetitionName.Should().Be(competition.Name);
        model.HasReviewedCriteria.Should().Be(competition.HasReviewedCriteria);
        model.NonPriceElementType.Should().Be(nonPriceElementType);
        model.NonPriceElementDetails.Should().BeEquivalentTo(competition.NonPriceElements);
        model.AvailableIntegrations.Should().BeNull();
        model.SingleElementDisplay.Should().Be(nonPriceElementType.EnumMemberName());
        model.Advice.Should()
            .Be(
                "Deleting this requirement will remove " + nonPriceElementType.EnumMemberName().ToLower()
                + " as a non-price element.");
    }

    [Theory]
    [MockAutoData]
    public static void Construct_SetsPropertiesAsExpected_Features(
        Organisation organisation,
        Competition competition,
        NonPriceElements nonPriceElements,
        ICollection<FeaturesCriteria> features,
        IEnumerable<Integration> availableIntegrations)
    {
        competition.Organisation = organisation;
        nonPriceElements.Features = features;
        competition.NonPriceElements = nonPriceElements;
        var model = new DeleteNonPriceElementModel(NonPriceElement.Features, competition, availableIntegrations, features.First().Id);

        model.NonPriceElementType.Should().Be(NonPriceElement.Features);
        model.NonPriceElementDetails.Features.Should().BeEquivalentTo(competition.NonPriceElements.Features.Where(x => x.Id == features.First().Id).ToList());
        model.AvailableIntegrations.Should().BeEquivalentTo(availableIntegrations.ToDictionary(x => x.Id, x => x.Name));
        model.SingleElementDisplay.Should().Be("Feature");
        model.Advice.Should()
            .Be(
                "If you delete all your features requirements, features will be removed as a non-price element.");
    }
}
