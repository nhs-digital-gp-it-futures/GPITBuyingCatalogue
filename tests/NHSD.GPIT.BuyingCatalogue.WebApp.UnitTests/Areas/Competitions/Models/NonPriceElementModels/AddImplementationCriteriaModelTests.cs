using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.NonPriceElementModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Models.NonPriceElementModels;

public static class AddImplementationCriteriaModelTests
{
    [Theory]
    [CommonAutoData]
    public static void Construct_SetsPropertiesAsExpected(
        ImplementationCriteria implementationCriteria,
        NonPriceElements elements,
        Competition competition)
    {
        elements.Implementation = implementationCriteria;
        competition.NonPriceElements = elements;

        var model = new AddImplementationCriteriaModel(competition);

        model.CompetitionName.Should().Be(competition.Name);
        model.Requirements.Should().Be(implementationCriteria.Requirements);
    }
}
