using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.FilterModels;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.Shorlists;
using NSubstitute;
using Xunit;
namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Models.Shared;

public static class ReviewFilterModelTests
{
    [Theory]
    [MockAutoData]
    public static void Construct_ExpectedModel(
        FilterDetailsModel filterDetails,
        string internalOrgId,
        List<CatalogueItem> filterResults,
        Solution solution,
        bool inCompetition,
        FilterIdsModel filterIds)
    {
        filterResults.ForEach(x => x.Solution = solution);
        var result = new ReviewFilterModel(filterDetails, internalOrgId, filterResults, inCompetition, filterIds);

        result.ResultsCount.Should().Be(filterResults.Count);
        result.InCompetition.Should().Be(inCompetition);

        var groupedResults = filterResults
            .SelectMany(x => x.Solution.FrameworkSolutions)
            .GroupBy(x => (x.Framework.ShortName, x.Framework.Id));
        result.ResultsForFrameworks.Count.Should().Be(groupedResults.Count());
    }

}
