using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Models;

public static class ExpiredFrameworkModelTests
{
    [Theory]
    [MockAutoData]
    public static void GetWarningText_OneExpiredNoActive_ReturnsExpected(
        EntityFramework.Catalogue.Models.Framework expiredFramework)
    {
        expiredFramework.IsExpired = true;

        var frameworks = new List<EntityFramework.Catalogue.Models.Framework> { expiredFramework };

        var model = new ExpiredFrameworksModel(frameworks);

        model.GetWarningText().Should().Be(ExpiredFrameworksModel.SingleExpiredFramework);
    }

    [Theory]
    [MockAutoData]
    public static void GetWarningText_MultipleExpiredNoActive_ReturnsExpected(
        List<EntityFramework.Catalogue.Models.Framework> frameworks)
    {
        frameworks.ForEach(x => x.IsExpired = true);

        var model = new ExpiredFrameworksModel(frameworks);

        model.GetWarningText().Should().Be(ExpiredFrameworksModel.MultipleExpiredFrameworks);
    }

    [Theory]
    [MockAutoData]
    public static void GetWarningText_OneExpiredWithActive_ReturnsExpected(
        EntityFramework.Catalogue.Models.Framework expiredFramework,
        EntityFramework.Catalogue.Models.Framework activeFramework)
    {
        expiredFramework.IsExpired = true;
        activeFramework.IsExpired = false;

        var frameworks = new List<EntityFramework.Catalogue.Models.Framework> { expiredFramework, activeFramework };

        var model = new ExpiredFrameworksModel(frameworks);

        model.GetWarningText()
            .Should()
            .Be(string.Format(ExpiredFrameworksModel.SingleExpiredWithActiveFrameworks, expiredFramework.ShortName));
    }

    [Theory]
    [MockAutoData]
    public static void GetWarningText_MultipleExpiredWithActive_ReturnsExpected(
        List<EntityFramework.Catalogue.Models.Framework> frameworks)
    {
        var expiredFrameworks = frameworks.Take(2).ToList();
        expiredFrameworks.ForEach(x => x.IsExpired = true);

        frameworks.Skip(2).ToList().ForEach(x => x.IsExpired = false);

        var model = new ExpiredFrameworksModel(frameworks);

        model.GetWarningText()
            .Should()
            .Be(
                string.Format(
                    ExpiredFrameworksModel.MultipleExpiredWithActiveFrameworks,
                    string.Join(", ", expiredFrameworks.Select(x => x.ShortName))));
    }
}
