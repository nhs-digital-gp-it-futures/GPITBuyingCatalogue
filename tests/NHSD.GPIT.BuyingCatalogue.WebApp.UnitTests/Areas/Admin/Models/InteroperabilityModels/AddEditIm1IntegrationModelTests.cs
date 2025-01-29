using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.InteroperabilityModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models.InteroperabilityModels;

public static class AddEditIm1IntegrationModelTests
{
    [Theory]
    [MockAutoData]
    public static void Construct_AddScenario_SetsPropertiesAsExpected(
        Solution solution,
        ICollection<IntegrationType> integrationTypes)
    {
        var expectedIntegrationTypes = integrationTypes.Select(x => new SelectOption<string>(x.Name, x.Id.ToString())).ToList();

        var model = new AddEditIm1IntegrationModel(solution.CatalogueItem, integrationTypes);

        Assert.Equal(solution.CatalogueItemId, model.SolutionId);
        Assert.Equal(solution.CatalogueItem.Name, model.SolutionName);
        Assert.Equivalent(expectedIntegrationTypes, model.IntegrationTypes);
    }

    [Theory]
    [MockAutoData]
    public static void Construct_EditScenario_SetsPropertiesAsExpected(
        Solution solution,
        ICollection<IntegrationType> integrationTypes,
        SolutionIntegration solutionIntegration)
    {
        var expectedIntegrationTypes = integrationTypes.Select(x => new SelectOption<string>(x.Name, x.Id.ToString())).ToList();

        var model = new AddEditIm1IntegrationModel(solution.CatalogueItem, integrationTypes, solutionIntegration);

        Assert.Equal(solution.CatalogueItemId, model.SolutionId);
        Assert.Equal(solution.CatalogueItem.Name, model.SolutionName);
        Assert.Equivalent(expectedIntegrationTypes, model.IntegrationTypes);
        Assert.Equal(solutionIntegration.Id, model.IntegrationId);
        Assert.Equal(solutionIntegration.IntegrationTypeId, model.SelectedIntegrationType);
        Assert.Equal(solutionIntegration.IsConsumer, model.IsConsumer);
        Assert.Equal(solutionIntegration.Description, model.Description);
        Assert.Equal(solutionIntegration.IntegratesWith, model.IntegratesWith);
    }
}
