using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Solutions.Models
{
    public static class SolutionStandardsExpanderModelTests
    {
        [Theory]
        [MockAutoData]
        public static void Constructor_PopulatesAllProperties(string title, string content, string testId, IList<StandardComplianceModel> standards, IList<string> standardsWithWorkOffPlans, CatalogueItemId solutionId, bool showAction)
        {
            var model = new SolutionStandardsExpanderModel(
                title,
                content,
                testId,
                standards,
                standardsWithWorkOffPlans,
                solutionId,
                showAction);

            model.Title.Should().Be(title);
            model.Content.Should().Be(content);
            model.TestId.Should().Be(testId);
            model.Standards.Should().BeEquivalentTo(standards);
            model.StandardsWithWorkOffPlans.Should().BeEquivalentTo(standardsWithWorkOffPlans);
            model.SolutionId.Should().Be(solutionId);
            model.ShowAction.Should().Be(showAction);
        }
    }
}
