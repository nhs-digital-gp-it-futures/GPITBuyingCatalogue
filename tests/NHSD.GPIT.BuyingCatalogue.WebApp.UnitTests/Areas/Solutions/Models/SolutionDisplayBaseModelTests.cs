using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture.NUnit3;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Solutions.Models
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class SolutionDisplayBaseModelTests
    {
        [TestCase(typeof(ClientApplicationTypesModel))]
        [TestCase(typeof(ImplementationTimescalesModel))]
        [TestCase(typeof(SolutionDescriptionModel))]
        public static void ChildClasses_InheritFrom_SolutionDisplayBaseModel(Type childType)
        {
            typeof(SolutionDescriptionModel)
                .Should()
                .BeAssignableTo<SolutionDisplayBaseModel>();
        }

        [TestCase("DESCRIPTION")]
        [TestCase("description")]
        [TestCase("desCRIPtion")]
        public static void GetSectionForDescription_SectionValid_ReturnsExpectedSectionModel(string section)
        {
            var model = new TestMarketingDisplayBaseModel();

            var actual = model.GetSectionFor(section);

            actual.Should()
                .BeEquivalentTo(
                    new SectionModel { Action = "Description", Controller = "SolutionDetails", Name = "Description", });
        }
        
        [TestCase("FEATURES")]
        [TestCase("features")]
        [TestCase("feaTUres")]
        public static void GetSectionForFeatures_SectionValid_ReturnsExpectedSectionModel(string section)
        {
            var model = new TestMarketingDisplayBaseModel();

            var actual = model.GetSectionFor(section);

            actual.Should()
                .BeEquivalentTo(
                    new SectionModel { Action = "Features", Controller = "SolutionDetails", Name = "Features", });
        }

        [TestCase("Description","Description")]
        [TestCase("Features", "Features")]
        public static void GetSectionFor_SectionValid_ReturnsExpectedSectionModel(string action, string section)
        {
            var model = new TestMarketingDisplayBaseModel();

            var actual = model.GetSectionFor(section);

            actual.Should()
                .BeEquivalentTo(
                    new SectionModel { Action = action, Controller = "SolutionDetails", Name = section, });
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("    ")]
        [TestCase("abc123")]
        public static void GetSectionFor_SectionInvalid_ReturnsExpectedSectionModel(string section)
        {
            var model = new TestMarketingDisplayBaseModel();

            var actual = model.GetSectionFor(section);

            actual.Should().BeNull();
        }

        [AutoData]
        [Test]
        public static void GetSections_ValidSectionProperty_ReturnsSectionsWithSelected(string solutionId)
        {
            var model = new TestMarketingDisplayBaseModel
            {
                Section = "implementation TIMESCALES", SolutionId = solutionId,
            };
            var expected = new List<SectionModel>(SectionModels);
            expected.ForEach(s => s.Id = solutionId);
            expected.Single(s => s.Name.EqualsIgnoreCase(model.Section)).Selected = true;

            var actual = model.GetSections();

            actual.Should().BeEquivalentTo(expected);
        }

        public class TestMarketingDisplayBaseModel : SolutionDisplayBaseModel
        {
            public override string Section { get; set; }
        }

        private static readonly IList<SectionModel> SectionModels = new List<SectionModel>
        {
            new()
            {
                Action = nameof(SolutionDetailsController.Description),
                Controller = typeof(SolutionDetailsController).ControllerName(),
                Name = "Description",
            },
            new()
            {
                Action = nameof(SolutionDetailsController.Features),
                Controller = typeof(SolutionDetailsController).ControllerName(),
                Name = "Features",
            },
            new()
            {
                Action = nameof(SolutionDetailsController.Description),
                Controller = typeof(SolutionDetailsController).ControllerName(),
                Name = "Capabilities",
            },
            new()
            {
                Action = nameof(SolutionDetailsController.Description),
                Controller = typeof(SolutionDetailsController).ControllerName(),
                Name = "List price",
            },
            new()
            {
                Action = nameof(SolutionDetailsController.Description),
                Controller = typeof(SolutionDetailsController).ControllerName(),
                Name = "Additional Services",
            },
            new()
            {
                Action = nameof(SolutionDetailsController.Description),
                Controller = typeof(SolutionDetailsController).ControllerName(),
                Name = "Associated Services",
            },
            new()
            {
                Action = nameof(SolutionDetailsController.Description),
                Controller = typeof(SolutionDetailsController).ControllerName(),
                Name = "Interoperability",
            },
            new()
            {
                Action = nameof(SolutionDetailsController.ImplementationTimescales),
                Controller = typeof(SolutionDetailsController).ControllerName(),
                Name = "Implementation timescales",
            },
            new()
            {
                Action = nameof(SolutionDetailsController.Description),
                Controller = typeof(SolutionDetailsController).ControllerName(),
                Name = "Client application type",
            },
            new()
            {
                Action = nameof(SolutionDetailsController.Description),
                Controller = typeof(SolutionDetailsController).ControllerName(),
                Name = "Hosting type",
            },
            new()
            {
                Action = nameof(SolutionDetailsController.Description),
                Controller = typeof(SolutionDetailsController).ControllerName(),
                Name = "Service Level Agreement",
            },
            new()
            {
                Action = nameof(SolutionDetailsController.Description),
                Controller = typeof(SolutionDetailsController).ControllerName(),
                Name = "Development plans",
            },
            new()
            {
                Action = nameof(SolutionDetailsController.Description),
                Controller = typeof(SolutionDetailsController).ControllerName(),
                Name = "Supplier details",
            },
        };
    }
}
