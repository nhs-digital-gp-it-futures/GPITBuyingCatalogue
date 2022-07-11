using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentValidation.TestHelper;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.ServiceLevelAgreements;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ServiceLevelAgreements;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.ServiceLevelAgreementValidators;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators
{
    public static class AddEditServiceLevelModelValidatorTests
    {
        [Theory]
        [CommonAutoData]
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
        [CommonAutoData]
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
        [CommonAutoData]
        public static void Validate_WithNoHowMeasured_SetsModelError(
            AddEditServiceLevelModelValidator validator)
        {
            var model = new AddEditServiceLevelModel
            {
                ServiceType = "Service Type",
                ServiceLevel = "Service Level",
                CreditsApplied = true,
            };

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.HowMeasured)
                .WithErrorMessage("Enter how service levels are measured");
        }

        [Theory]
        [CommonAutoData]
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
        [CommonAutoData]
        public static void Validate_Duplicate_SetsModelError(
            Solution solution,
            ServiceLevelAgreements serviceLevelAgreement,
            SlaServiceLevel serviceLevel,
            [Frozen] Mock<IServiceLevelAgreementsService> serviceLevelAgreementsService,
            AddEditServiceLevelModelValidator validator)
        {
            serviceLevelAgreement.ServiceLevels.Add(serviceLevel);

            serviceLevelAgreementsService.Setup(s => s.GetServiceLevelAgreementForSolution(solution.CatalogueItemId))
                .ReturnsAsync(serviceLevelAgreement);

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
