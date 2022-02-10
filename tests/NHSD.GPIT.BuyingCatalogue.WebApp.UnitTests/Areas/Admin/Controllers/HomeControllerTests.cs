using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Controllers
{
    public static class HomeControllerTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(HomeController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_BuyerOrganisations_GetsAllOrganisations(
            IList<Organisation> organisations,
            [Frozen] Mock<IOrganisationsService> mockOrganisationService,
            HomeController controller)
        {
            mockOrganisationService.Setup(o => o.GetAllOrganisations())
                .ReturnsAsync(organisations);

            await controller.BuyerOrganisations();

            mockOrganisationService.Verify(o => o.GetAllOrganisations());
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_BuyerOrganisations_ReturnsViewWithExpectedViewModel(
            IList<Organisation> organisations,
            [Frozen] Mock<IOrganisationsService> mockOrganisationService,
            HomeController controller)
        {
            mockOrganisationService.Setup(o => o.GetAllOrganisations())
                .ReturnsAsync(organisations);

            var expectedOrganisationModels = organisations.Select(
                o => new OrganisationModel
                {
                    Id = o.Id,
                    Name = o.Name,
                    OdsCode = o.InternalIdentifier,
                }).ToList();

            var actual = (await controller.BuyerOrganisations()).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.As<ListOrganisationsModel>().Organisations.Should().BeEquivalentTo(expectedOrganisationModels);
        }

        [Theory]
        [CommonAutoData]
        public static void Get_Index_ReturnsDefaultView(
            HomeController controller)
        {
            var result = controller.Index().As<ViewResult>();

            result.Should().NotBeNull();
            result.ViewName.Should().BeNull();
        }
    }
}
