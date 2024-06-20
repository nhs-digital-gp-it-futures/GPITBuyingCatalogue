using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentValidation.TestHelper;
using Microsoft.CodeAnalysis;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.ServiceLevelAgreements;
using NHSD.GPIT.BuyingCatalogue.Services.ServiceLevelAgreements;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ServiceLevelAgreements;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.ServiceLevelAgreementValidators;
using NSubstitute;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators
{
    public static class AddEditServiceLevelModelValidatorTests
    {
        [Theory]
        [MockAutoData]
        public static void Validate_WithNoServiceType_SetsModelError(
            AddEditServiceLevelModelValidator validator)
        {
            var model = new AddEditServiceLevelModel
            {
                ServiceLevel = "Service Level",
                HowMeasured = "How Measured",
                CreditsApplied = true,
            };

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.ServiceType)
                .WithErrorMessage("Enter a type of service");
        }

        [Theory]
        [MockAutoData]
        public static void Validate_WithNoServiceLevel_SetsModelError(
            AddEditServiceLevelModelValidator validator)
        {
            var model = new AddEditServiceLevelModel
            {
                ServiceType = "Service Type",
                HowMeasured = "How Measured",
                CreditsApplied = true,
            };

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.ServiceLevel)
                .WithErrorMessage("Enter a service level");
        }

        [Theory]
        [MockAutoData]
        public static void Validate_WithNoHowMeasured_SetsModelError(
            AddEditServiceLevelModelValidator validator,
            [Frozen] IServiceLevelAgreementsService serviceLevelAgreementsService,
            ServiceLevelAgreements serviceLevelAgreements,
            EntityFramework.Catalogue.Models.Solution solution)
        {
            var model = new AddEditServiceLevelModel
            {
                ServiceType = "Service Type",
                ServiceLevel = "Service Level",
                CreditsApplied = true,
            };
            var test1 = Task.FromResult(serviceLevelAgreements);
            //var serviceLevelAgreementsService = Substitute.For<ServiceLevelAgreementsService>();
            serviceLevelAgreementsService.GetServiceLevelAgreementForSolution(Arg.Any<CatalogueItemId>()).Returns(Task.FromResult(Substitute.For<ServiceLevelAgreements>()));
            //serviceLevelAgreementsService.GetServiceLevelAgreementForSolution(Arg.Any<CatalogueItemId>()).Returns(solution.ServiceLevelAgreement);

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.HowMeasured)
                .WithErrorMessage("Enter how service levels are measured");
        }

        [Theory]
        [MockAutoData]
        public static void Validate_WithNoCreditsSelection_SetsModelError(
            AddEditServiceLevelModelValidator validator)
        {
            var model = new AddEditServiceLevelModel
            {
                ServiceType = "Service Type",
                ServiceLevel = "Service Level",
                HowMeasured = "How Measured",
            };

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.CreditsApplied)
                .WithErrorMessage("Select if service credits are applied");
        }

        [Theory]
        [MockAutoData]
        public static void Validate_Duplicate_SetsModelError(
            EntityFramework.Catalogue.Models.Solution solution,
            ServiceLevelAgreements serviceLevelAgreement,
            SlaServiceLevel serviceLevel,
            [Frozen] IServiceLevelAgreementsService serviceLevelAgreementsService,
            AddEditServiceLevelModelValidator validator)
        {
            serviceLevelAgreement.ServiceLevels.Add(serviceLevel);

            serviceLevelAgreementsService.GetServiceLevelAgreementForSolution(solution.CatalogueItemId).Returns(serviceLevelAgreement);

            var model = new AddEditServiceLevelModel(solution.CatalogueItem)
            {
                ServiceType = serviceLevel.TypeOfService,
                ServiceLevel = serviceLevel.ServiceLevel,
                HowMeasured = serviceLevel.HowMeasured,
                CreditsApplied = serviceLevel.ServiceCredits,
            };

            var result = validator.TestValidate(model);

            result.ShouldHaveAnyValidationError()
                .WithErrorMessage("Service level with these details already exists");
        }
    }
}
