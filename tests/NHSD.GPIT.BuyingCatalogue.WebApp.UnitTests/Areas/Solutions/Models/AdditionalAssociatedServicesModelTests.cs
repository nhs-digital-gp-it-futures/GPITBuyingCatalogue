using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Solutions.Models;

public static class AdditionalAssociatedServicesModelTests
{
    [Theory]
    [MockAutoData]
    public static void Construct_SetsProperties(
        Solution solution,
        AdditionalService additionalService,
        List<AssociatedService> associatedServices,
        CatalogueItemContentStatus contentStatus)
    {
        var associatedServiceCatalogueItems = associatedServices.Select(x => x.CatalogueItem).ToList();

        var model = new AdditionalAssociatedServicesModel(solution.CatalogueItem, additionalService.CatalogueItem, associatedServiceCatalogueItems, contentStatus);

        model.Title.Should().Be("Associated services");
        model.Caption.Should().Be(additionalService.CatalogueItem.Name);
        model.Index.Should().BeGreaterOrEqualTo(4);
        model.IsSubPage.Should().BeTrue();
    }

    [Theory]
    [MockAutoData]
    public static void GetPricePageUrl_RetrievesExpectedUrl(
        Solution solution,
        AdditionalService additionalService,
        List<AssociatedService> associatedServices,
        CatalogueItemContentStatus contentStatus,
        IUrlHelper urlHelper)
    {
        var associatedServiceCatalogueItems = associatedServices.Select(x => x.CatalogueItem).ToList();
        var associatedService = associatedServiceCatalogueItems.First();

        var model = new AdditionalAssociatedServicesModel(solution.CatalogueItem, additionalService.CatalogueItem, associatedServiceCatalogueItems, contentStatus);

        _ = model.GetPricePageUrl(urlHelper, associatedService.Id);

        urlHelper.Received()
            .Action(
                Arg.Is<UrlActionContext>(x =>
                    x.Action == nameof(SolutionsController.AssociatedServicePrice)
                    && x.Controller == typeof(SolutionsController).ControllerName()));
    }
}
