using System;
using System.Collections.Generic;
using FluentAssertions;
using Moq;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
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
                Action = nameof(SolutionsController.ApplicationTypes),
                Controller = typeof(SolutionsController).ControllerName(),
                Name = "Application type",
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
        [InlineData(typeof(ApplicationTypesModel))]
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
        [InlineData("Summary", false)]
        [InlineData("summary", false)]
        [InlineData("SUMMARY", false)]
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
    }
}
