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
        [TestCase(typeof(SolutionFeaturesModel))]
        public static void ChildClasses_InheritFrom_SolutionDisplayBaseModel(Type childType)
        {
            childType
            .Should()
            .BeAssignableTo<SolutionDisplayBaseModel>();
        }
        
        [AutoData]
        [Test]
        public static void GetSections_ValidSectionProperty_ReturnsSectionsWithSelected(string solutionId)
        {
            var model = new TestSolutionDisplayBaseModel { SolutionId = solutionId, };
            for (int i = 0; i < 12; i++)
            {
                if (i % 2 != 0) continue;
                model.SetShowTrue(i);
                SectionModels[i].Show = true;
            }
            
            var expected = new List<SectionModel>(SectionModels.Where(s => s.Show));
            expected.ForEach(s => s.Id = solutionId);
            expected.Single(s => s.Name.EqualsIgnoreCase(model.Section)).Selected = true;
            
            var actual = model.GetSections();

            actual.Should().BeEquivalentTo(expected);
        }
        
        public static readonly IList<SectionModel> SectionModels = new List<SectionModel>
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
                Action = nameof(SolutionDetailsController.Capabilities),
                Controller = typeof(SolutionDetailsController).ControllerName(),
                Name = "Capabilities",
            },
            new()
            {
                Action = nameof(SolutionDetailsController.ListPrice),
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
                Action = nameof(SolutionDetailsController.Interoperability),
                Controller = typeof(SolutionDetailsController).ControllerName(),
                Name = "Interoperability",
            },
            new()
            {
                Action = nameof(SolutionDetailsController.Implementation),
                Controller = typeof(SolutionDetailsController).ControllerName(),
                Name = "Implementation",
            },
            new()
            {
                Action = nameof(SolutionDetailsController.ClientApplicationTypes),
                Controller = typeof(SolutionDetailsController).ControllerName(),
                Name = "Client application type",
            },
            new()
            {
                Action = nameof(SolutionDetailsController.HostingType),
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
