using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.FilterModels;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.ActionFilters;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Controllers
{
    public static class SolutionsFilterResultsControllerTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(SolutionsFilterResultsController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Fact]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(SolutionsFilterResultsController).Should().BeDecoratedWith<RestrictToLocalhostActionFilter>();
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Index_Null_Filter_Returns_NotFound(
            string internalOrgId,
            int filterId,
            FilterIdsModel filterIds,
            [Frozen] Organisation organisation,
            [Frozen] Mock<IOrganisationsService> organisationsService,
            [Frozen] Mock<IManageFiltersService> manageFilterService,
            SolutionsFilterResultsController controller)
        {
            organisationsService
                .Setup(s => s.GetOrganisationByInternalIdentifier(It.IsAny<string>()))
                .ReturnsAsync(organisation);

            manageFilterService
                .Setup(s => s.GetFilterDetails(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync((FilterDetailsModel)null);

            manageFilterService
                .Setup(s => s.GetFilterIds(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(filterIds);

            var result = await controller.Index(internalOrgId, filterId);
            result.Should().BeOfType<NotFoundResult>();
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Index_Null_FilterIds_Returns_NotFound(
            string internalOrgId,
            int filterId,
            FilterDetailsModel filter,
            [Frozen] Organisation organisation,
            [Frozen] Mock<IOrganisationsService> organisationsService,
            [Frozen] Mock<IManageFiltersService> manageFilterService,
            SolutionsFilterResultsController controller)
        {
            organisationsService
                .Setup(s => s.GetOrganisationByInternalIdentifier(It.IsAny<string>()))
                .ReturnsAsync(organisation);

            manageFilterService
                .Setup(s => s.GetFilterDetails(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(filter);

            manageFilterService
                .Setup(s => s.GetFilterIds(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync((FilterIdsModel)null);

            var result = await controller.Index(internalOrgId, filterId);
            result.Should().BeOfType<NotFoundResult>();
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Index_Returns_ViewResult(
            string internalOrgId,
            int filterId,
            FilterDetailsModel filter,
            FilterIdsModel filterIds,
            [Frozen] Organisation organisation,
            [Frozen] Mock<IOrganisationsService> organisationsService,
            [Frozen] Mock<IManageFiltersService> manageFilterService,
            SolutionsFilterResultsController controller)
        {
            organisationsService
                .Setup(s => s.GetOrganisationByInternalIdentifier(It.IsAny<string>()))
                .ReturnsAsync(organisation);

            manageFilterService
                .Setup(s => s.GetFilterDetails(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(filter);

            manageFilterService
                .Setup(s => s.GetFilterIds(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(filterIds);

            var result = await controller.Index(internalOrgId, filterId);
            result.Should().BeOfType<ViewResult>();
        }
    }
}
