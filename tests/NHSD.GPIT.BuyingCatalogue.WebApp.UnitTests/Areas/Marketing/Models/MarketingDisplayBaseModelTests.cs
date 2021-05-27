using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture.NUnit3;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Marketing.Models
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class MarketingDisplayBaseModelTests
    {
        [TestCase("FEATURES")]
        [TestCase("features")]
        [TestCase("feaTURes")]
        public static void GetSectionFor_SectionValid_ReturnsExpectedSectionModel(string section)
        {
            var model = new TestMarketingDisplayBaseModel();

            var actual = model.GetSectionFor(section);
            
            actual.Should().BeEquivalentTo(new SectionModel
            {
                Action = "Features",
                Controller = "AboutSolution",
                Name = "Features",
            });
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
                Section = "implementation TIMESCALES",
                SolutionId = solutionId,
            };
            var expected = new List<SectionModel>(SectionModels);
            expected.ForEach(s => s.Id = solutionId);
            expected.Single(s => s.Name.EqualsIgnoreCase(model.Section)).Selected = true;

            var actual = model.GetSections();
            
            actual.Should().BeEquivalentTo(expected);
        }
        
        public class TestMarketingDisplayBaseModel : MarketingDisplayBaseModel
        {
            public override DateTime LastReviewed { get; set; }
            
            public override PaginationFooterModel PaginationFooter { get; set; }
            
            public override string Section { get; set; }
            
            public override string SolutionId { get; set; }
        }

        private static readonly IList<SectionModel> SectionModels = new List<SectionModel>
        {
            new()
            {
                Action = nameof(SolutionController.Index),
                Controller = typeof(SolutionController).ControllerName(),
                Name = "Description",
            },
            new()
            {
                Action = nameof(AboutSolutionController.Features),
                Controller = typeof(AboutSolutionController).ControllerName(),
                Name = "Features",
            },
            new()
            {
                Action = nameof(SolutionController.Index),
                Controller = typeof(SolutionController).ControllerName(),
                Name = "Capabilities",
            },
            new()
            {
                Action = nameof(SolutionController.Index),
                Controller = typeof(SolutionController).ControllerName(),
                Name = "List price",
            },
            new()
            {
                Action = nameof(SolutionController.Index),
                Controller = typeof(SolutionController).ControllerName(),
                Name = "Additional Services",
            },
            new()
            {
                Action = nameof(SolutionController.Index),
                Controller = typeof(SolutionController).ControllerName(),
                Name = "Associated Services",
            },
            new()
            {
                Action = nameof(SolutionController.Index),
                Controller = typeof(SolutionController).ControllerName(),
                Name = "Interoperability",
            },
            new()
            {
                Action = nameof(AboutSolutionController.ImplementationTimescales),
                Controller = typeof(AboutSolutionController).ControllerName(),
                Name = "Implementation timescales",
            },
            new()
            {
                Action = nameof(ClientApplicationTypeController.ClientApplicationTypes),
                Controller = typeof(ClientApplicationTypeController).ControllerName(),
                Name = "Client application type",
            },
            new()
            {
                Action = nameof(SolutionController.Index),
                Controller = typeof(SolutionController).ControllerName(),
                Name = "Hosting type",
            },
            new()
            {
                Action = nameof(SolutionController.Index),
                Controller = typeof(SolutionController).ControllerName(),
                Name = "Service Level Agreement",
            },
            new()
            {
                Action = nameof(SolutionController.Index),
                Controller = typeof(SolutionController).ControllerName(),
                Name = "Development plans",
            },
            new()
            {
                Action = nameof(SolutionController.Index),
                Controller = typeof(SolutionController).ControllerName(),
                Name = "Supplier details",
            },
        };
    }
}
