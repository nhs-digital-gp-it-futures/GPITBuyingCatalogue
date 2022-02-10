using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.OrganisationModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Autocomplete;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Controllers
{
    public static class OrganisationsControllerTests
    {
        [Fact]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(OrganisationsController).Should().BeDecoratedWith<AuthorizeAttribute>(a => a.Policy == "AdminOnly");
            typeof(OrganisationsController).Should().BeDecoratedWith<AreaAttribute>(a => a.RouteValue == "Admin");
        }

        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(OrganisationsController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Index_GetsAllOrganisations(
            IList<Organisation> organisations,
            [Frozen] Mock<IOrganisationsService> mockOrganisationService,
            OrganisationsController controller)
        {
            mockOrganisationService
                .Setup(o => o.GetAllOrganisations())
                .ReturnsAsync(organisations);

            await controller.Index();

            mockOrganisationService.VerifyAll();
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Index_ReturnsViewWithExpectedViewModel(
            IList<Organisation> organisations,
            [Frozen] Mock<IOrganisationsService> mockOrganisationService,
            OrganisationsController controller)
        {
            mockOrganisationService
                .Setup(o => o.GetAllOrganisations())
                .ReturnsAsync(organisations);

            var model = organisations
                .Select(o => new OrganisationModel
                {
                    Id = o.Id,
                    Name = o.Name,
                    OdsCode = o.OdsCode,
                })
                .ToList();

            var actual = (await controller.Index()).As<ViewResult>();

            mockOrganisationService.VerifyAll();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.As<IndexModel>().Organisations.Should().BeEquivalentTo(model);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Index_ValidSearchTerm_ReturnsViewWithExpectedViewModel(
            string searchTerm,
            List<Organisation> organisations,
            [Frozen] Mock<IOrganisationsService> mockOrganisationService,
            OrganisationsController controller)
        {
            mockOrganisationService
                .Setup(o => o.GetOrganisationsBySearchTerm(searchTerm))
                .ReturnsAsync(organisations);

            var model = organisations
                .Select(o => new OrganisationModel
                {
                    Id = o.Id,
                    Name = o.Name,
                    OdsCode = o.OdsCode,
                })
                .ToList();

            var actual = (await controller.Index(searchTerm)).As<ViewResult>();

            mockOrganisationService.VerifyAll();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.As<IndexModel>().Organisations.Should().BeEquivalentTo(model);
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        [CommonInlineAutoData(" ")]
        public static async Task Get_Index_InvalidSearchTerm_ReturnsViewWithExpectedViewModel(
            string searchTerm,
            List<Organisation> organisations,
            [Frozen] Mock<IOrganisationsService> mockOrganisationService,
            OrganisationsController controller)
        {
            mockOrganisationService
                .Setup(o => o.GetAllOrganisations())
                .ReturnsAsync(organisations);

            var model = organisations
                .Select(o => new OrganisationModel
                {
                    Id = o.Id,
                    Name = o.Name,
                    OdsCode = o.OdsCode,
                })
                .ToList();

            var actual = (await controller.Index(searchTerm)).As<ViewResult>();

            mockOrganisationService.VerifyAll();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.As<IndexModel>().Organisations.Should().BeEquivalentTo(model);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_SearchResults_ValidSearchTerm_ReturnsExpectedResult(
            List<Organisation> organisations,
            string searchTerm,
            [Frozen] Mock<IOrganisationsService> mockOrganisationService,
            OrganisationsController controller)
        {
            mockOrganisationService
                .Setup(o => o.GetOrganisationsBySearchTerm(searchTerm))
                .ReturnsAsync(organisations);

            var result = await controller.SearchResults(searchTerm);

            mockOrganisationService.VerifyAll();

            var actualResult = result.As<JsonResult>()
                .Value.As<IEnumerable<AutocompleteResult>>()
                .ToList();

            foreach (var org in organisations)
            {
                actualResult.Should().Contain(x => x.Title == org.Name && x.Category == org.OdsCode);
            }
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_SearchResults_ValidSearchTerm_NoMatches_ReturnsExpectedResult(
            string searchTerm,
            [Frozen] Mock<IOrganisationsService> mockOrganisationService,
            OrganisationsController controller)
        {
            mockOrganisationService
                .Setup(o => o.GetOrganisationsBySearchTerm(searchTerm))
                .ReturnsAsync(new List<Organisation>());

            var result = await controller.SearchResults(searchTerm);

            mockOrganisationService.VerifyAll();

            var actualResult = result.As<JsonResult>()
                .Value.As<IEnumerable<AutocompleteResult>>()
                .ToList();

            actualResult.Should().BeEmpty();
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        [CommonInlineAutoData(" ")]
        public static async Task Get_SearchResults_InvalidSearchTerm_ReturnsExpectedResult(
            string searchTerm,
            OrganisationsController controller)
        {
            var result = await controller.SearchResults(searchTerm);

            var actualResult = result.As<JsonResult>()
                .Value.As<IEnumerable<AutocompleteResult>>()
                .ToList();

            actualResult.Should().BeEmpty();
        }
    }
}
