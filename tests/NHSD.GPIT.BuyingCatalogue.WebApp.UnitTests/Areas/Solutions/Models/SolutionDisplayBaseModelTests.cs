using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture.Xunit2;
using FluentAssertions;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Solutions.Models
{
    public static class SolutionDisplayBaseModelTests
    {
        private static readonly IList<SectionModel> SectionModels = new List<SectionModel>
        {
            new()
            {
                Action = nameof(SolutionsController.Description),
                Controller = typeof(SolutionsController).ControllerName(),
                Name = "Description",
            },
            new()
            {
                Action = nameof(SolutionsController.Features),
                Controller = typeof(SolutionsController).ControllerName(),
                Name = "Features",
            },
            new()
            {
                Action = nameof(SolutionsController.Capabilities),
                Controller = typeof(SolutionsController).ControllerName(),
                Name = "Capabilities",
            },
            new()
            {
                Action = nameof(SolutionsController.ListPrice),
                Controller = typeof(SolutionsController).ControllerName(),
                Name = "List price",
            },
            new()
            {
                Action = nameof(SolutionsController.AdditionalServices),
                Controller = typeof(SolutionsController).ControllerName(),
                Name = "Additional Services",
            },
            new()
            {
                Action = nameof(SolutionsController.Description),
                Controller = typeof(SolutionsController).ControllerName(),
                Name = "Associated Services",
            },
            new()
            {
                Action = nameof(SolutionsController.Interoperability),
                Controller = typeof(SolutionsController).ControllerName(),
                Name = "Interoperability",
            },
            new()
            {
                Action = nameof(SolutionsController.Implementation),
                Controller = typeof(SolutionsController).ControllerName(),
                Name = "Implementation",
            },
            new()
            {
                Action = nameof(SolutionsController.ClientApplicationTypes),
                Controller = typeof(SolutionsController).ControllerName(),
                Name = "Client application type",
            },
            new()
            {
                Action = nameof(SolutionsController.HostingType),
                Controller = typeof(SolutionsController).ControllerName(),
                Name = "Hosting type",
            },
            new()
            {
                Action = nameof(SolutionsController.Description),
                Controller = typeof(SolutionsController).ControllerName(),
                Name = "Service Level Agreement",
            },
            new()
            {
                Action = nameof(SolutionsController.Description),
                Controller = typeof(SolutionsController).ControllerName(),
                Name = "Development plans",
            },
            new()
            {
                Action = nameof(SolutionsController.Description),
                Controller = typeof(SolutionsController).ControllerName(),
                Name = "Supplier details",
            },
        };

        [Theory]
        [InlineData(typeof(ClientApplicationTypesModel))]
        [InlineData(typeof(ImplementationTimescalesModel))]
        [InlineData(typeof(SolutionDescriptionModel))]
        [InlineData(typeof(SolutionFeaturesModel))]
        public static void ChildClasses_InheritFrom_SolutionDisplayBaseModel(Type childType)
        {
            childType
            .Should()
            .BeAssignableTo<SolutionDisplayBaseModel>();
        }

        [Theory]
        [CommonAutoData]
        public static void GetSections_ValidSectionProperty_ReturnsSectionsWithSelected(CatalogueItemId solutionId)
        {
            var model = new TestSolutionDisplayBaseModel { SolutionId = solutionId, };
            for (int i = 0; i < 12; i++)
            {
                if (i % 2 != 0) continue;
                model.SetShowTrue(i);
                SectionModels[i].Show = true;
            }

            var expected = new List<SectionModel>(SectionModels.Where(s => s.Show));
            expected.ForEach(s => s.SolutionId = solutionId.ToString());
            expected.Single(s => s.Name.EqualsIgnoreCase(model.Section)).Selected = true;

            var actual = model.GetSections();

            actual.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [InlineData("Description", false)]
        [InlineData("description", false)]
        [InlineData("DESCRIPTION", false)]
        [InlineData("Implementation", true)]
        [InlineData("Hosting", true)]
        public static void NotFirstSection_Returns_ExpectedResponse(string section, bool expected)
        {
            var model = new Mock<SolutionDisplayBaseModel> { CallBase = true };
            model.SetupGet(m => m.Section)
                .Returns(section);

            var actual = model.Object.NotFirstSection();

            model.VerifyGet(m => m.Section);
            actual.Should().Be(expected);
        }

        [Theory]
        [CommonInlineAutoData(PublicationStatus.Published, false)]
        [CommonInlineAutoData(PublicationStatus.InRemediation, true)]
        public static void IsInRemediation(
            PublicationStatus publicationStatus,
            bool expected,
            CatalogueItem item)
        {
            item.PublishedStatus = publicationStatus;

            var model = new TestSolutionDisplayBaseModel(item);

            model.IsInRemediation().Should().Be(expected);
        }
    }
}
