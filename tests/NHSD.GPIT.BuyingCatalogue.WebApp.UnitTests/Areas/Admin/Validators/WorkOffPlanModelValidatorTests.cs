using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.DevelopmentPlans;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.DevelopmentPlans;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.DevelopmentPlans;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators
{
    public static class WorkOffPlanModelValidatorTests
    {
        [Theory]
        [CommonInlineAutoData(null, null, null, null, null)]
        [CommonInlineAutoData("", "", "", "", "")]
        public static void Validate_InputsNullOrEmpty_SetsModelErrors(
            string selectStandard,
            string workOffPlansDetails,
            string dateDay,
            string dateMonth,
            string dateYear,
            EditWorkOffPlanModel model,
            EditWorkOffPlanValidator validator)
        {
            model.SelectedStandard = selectStandard;
            model.Details = workOffPlansDetails;
            model.Day = dateDay;
            model.Month = dateMonth;
            model.Year = dateYear;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.SelectedStandard)
                .WithErrorMessage(EditWorkOffPlanValidator.NoStandardSelectedError);

            result.ShouldHaveValidationErrorFor(m => m.Details)
                .WithErrorMessage(EditWorkOffPlanValidator.NoDetailsError);

            result.ShouldHaveValidationErrorFor(m => m.Day)
                .WithErrorMessage(EditWorkOffPlanValidator.NoDateDayError);
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        public static void Validate_NoMonth_SetsModelError(
            string dateMonth,
            EditWorkOffPlanModel model,
            EditWorkOffPlanValidator validator)
        {
            model.Month = dateMonth;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Day)
                .WithErrorMessage(EditWorkOffPlanValidator.NoDateMonthError);
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        public static void Validate_NoYear_SetsModelError(
            string dateYear,
            EditWorkOffPlanModel model,
            EditWorkOffPlanValidator validator)
        {
            model.Year = dateYear;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Day)
                .WithErrorMessage(EditWorkOffPlanValidator.NoDateYearError);
        }

        [Theory]
        [CommonInlineAutoData("1")]
        [CommonInlineAutoData("11111")]
        public static void Validate_YearNotLength4_SetsModelError(
           string dateYear,
           EditWorkOffPlanModel model,
           EditWorkOffPlanValidator validator)
        {
            model.Year = dateYear;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Day)
                .WithErrorMessage(EditWorkOffPlanValidator.DateErrorYearSize);
        }

        [Theory]
        [CommonInlineAutoData("50")]
        public static void Validate_IncorrectFormatDay_SetsModelError(
           string dateDay,
           EditWorkOffPlanModel model,
           EditWorkOffPlanValidator validator)
        {
            model.Day = dateDay;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Day)
                .WithErrorMessage(EditWorkOffPlanValidator.DateIncorrectFormatError);
        }

        [Theory]
        [CommonInlineAutoData("13")]
        public static void Validate_IncorrectFormatMonth_SetsModelError(
           string dateMonth,
           EditWorkOffPlanModel model,
           EditWorkOffPlanValidator validator)
        {
            model.Month = dateMonth;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Day)
                .WithErrorMessage(EditWorkOffPlanValidator.DateIncorrectFormatError);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_DateTooFarInPast_SetsModelError(
           EditWorkOffPlanModel model,
           EditWorkOffPlanValidator validator)
        {
            model.Day = DateTime.UtcNow.Day.ToString();
            model.Month = DateTime.UtcNow.Month.ToString();
            model.Year = DateTime.UtcNow.AddYears(-1).Year.ToString();

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Day)
                .WithErrorMessage(EditWorkOffPlanValidator.DateInPastError);
        }

        /*[Theory]
        [CommonAutoData]
        public static void Validate_DuplicateWorkOffPlan_SetsModelError(
            Mock<IDevelopmentPlansService> mockdevelopmentPlansService,
            WorkOffPlan model,
            EditWorkOffPlanModel workOffPlanModel,
            EditWorkOffPlanValidator validator)
        {

            var existingWorkOffPlan = mockdevelopmentPlansService.Setup(x => x.GetWorkOffPlan(workOffPlanModel.WorkOffPlanId.Value))
                .ReturnsAsync(model);


            var newModel = new EditWorkOffPlanModel()
            {
                SelectedStandard = model.Standard.ToString(),
                Details = model.Details,
                Day = model.CompletionDate.ToString().Split('/')[0],
                Month = model.CompletionDate.ToString().Split('/')[1],
                Year = model.CompletionDate.ToString().Split('/')[2],
            };

            var result = validator.TestValidate(newModel);

            result.ShouldHaveValidationErrorFor(m => m.Day)
                .WithErrorMessage(EditWorkOffPlanValidator.DateInPastError);
        }*/
    }
}
