using AutoFixture.Xunit2;
using FluentValidation.TestHelper;
using Moq;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.AssociatedServices;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.AssociatedServices;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators
{
    public static class EditAssociatedServiceDetailsModelValidatorTests
    {
        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        public static void Validate_NameNullOrEmpty_SetsModelError(
            string name,
            EditAssociatedServiceDetailsModel model,
            EditAssociatedServiceDetailsModelValidator validator)
        {
            model.Name = name;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Name)
                .WithErrorMessage("Enter a name");
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        public static void Validate_DescriptionNullOrEmpty_SetsModelError(
            string description,
            EditAssociatedServiceDetailsModel model,
            EditAssociatedServiceDetailsModelValidator validator)
        {
            model.Description = description;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Description)
                .WithErrorMessage("Enter a description");
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        public static void Validate_OrderGuidanceNullOrEmpty_SetsModelError(
            string orderGuidance,
            EditAssociatedServiceDetailsModel model,
            EditAssociatedServiceDetailsModelValidator validator)
        {
            model.OrderGuidance = orderGuidance;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.OrderGuidance)
                .WithErrorMessage("Enter order guidance");
        }

        // This test only tests that the system breaks with the generated test data, rather than testing
        // the actual behaviour.Creating a merger or split should not necessarily cause an error.
        [Theory]
        [CommonAutoData]
        public static void Validate_PracticeReoganisation_SetsModelError(
            EditAssociatedServiceDetailsModel model,
            EditAssociatedServiceDetailsModelValidator validator)
        {
            model.PracticeMerger = true;
            model.PracticeSplit = true;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor("practice-reorganisation");
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_DuplicateNameForSupplier_SetsModelError(
            [Frozen] Mock<IAssociatedServicesService> associatedServicesService,
            EditAssociatedServiceDetailsModel model,
            EditAssociatedServiceDetailsModelValidator validator)
        {
            associatedServicesService.Setup(s => s.AssociatedServiceExistsWithNameForSupplier(model.Name, model.SupplierId, model.Id.Value))
                .ReturnsAsync(true);

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Name)
                .WithErrorMessage("Associated Service name already exists. Enter a different name");
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_ValidName_NoModelError(
            [Frozen] Mock<IAssociatedServicesService> associatedServicesService,
            EditAssociatedServiceDetailsModel model,
            EditAssociatedServiceDetailsModelValidator validator)
        {
            associatedServicesService.Setup(s => s.AssociatedServiceExistsWithNameForSupplier(model.Name, model.SupplierId, default))
                .ReturnsAsync(false);

            model.PracticeMerger = false;
            model.PracticeSplit = false;

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [CommonInlineAutoData(true, true, true)]
        [CommonInlineAutoData(false, true, false)]
        [CommonInlineAutoData(true, false, false)]
        [CommonInlineAutoData(false, false, false)]
        public static void Validate_WithPriceReorganisation_CalculationProvisioningTypesAndTieredPrice(
            bool haveCorrectProvisioningAndCalculationTypes,
            bool notHaveTieredPrice,
            bool expected,
            // [Frozen] Mock<SolutionMergerAndSplitTypesModel> solutionMergerAndSplitTypesModelMock,
            // [Frozen] Mock<IAssociatedServicesService> associatedServicesService,
            /*[Frozen] BuyingCatalogueDbContext context,
            List<Solution> solutions,
            AssociatedService associatedService,*/
            EditAssociatedServiceDetailsModel model,
            EditAssociatedServiceDetailsModelValidator validator)
        {
            /*context.Solutions.AddRange(solutions);
            context.AssociatedServices.Add(associatedService);
            context.SaveChanges();
            context.ChangeTracker.Clear();*/
            model.PracticeSplit = true;
            model.PracticeSplit = false;
            model.HaveCorrectProvisioningAndCalculationTypes = haveCorrectProvisioningAndCalculationTypes;
            model.NotHaveTieredPrices = notHaveTieredPrice;

            var result = validator.TestValidate(model);

            if (expected)
            {
                result.ShouldNotHaveValidationErrorFor("practice-reorganisation");
            }
            else
            {
                result.ShouldHaveValidationErrorFor("practice-reorganisation")
                .WithErrorMessage("This Associated Service has invalid price types for mergers and splits. You must edit the price types first");
            }
        }
    }
}
