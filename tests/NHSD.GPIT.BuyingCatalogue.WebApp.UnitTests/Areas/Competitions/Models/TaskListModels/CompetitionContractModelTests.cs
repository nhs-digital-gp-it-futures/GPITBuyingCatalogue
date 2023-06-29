using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.TaskListModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Models.TaskListModels;

public sealed class CompetitionContractModelTests
{
    [Theory]
    [CommonAutoData]
    public static void Construct_SetsProperties(
        Competition competition)
    {
        var model = new CompetitionContractModel(competition);

        model.CompetitionName.Should().Be(competition.Name);
        model.ContractLength.Should().Be(competition.ContractLength);
    }
}
