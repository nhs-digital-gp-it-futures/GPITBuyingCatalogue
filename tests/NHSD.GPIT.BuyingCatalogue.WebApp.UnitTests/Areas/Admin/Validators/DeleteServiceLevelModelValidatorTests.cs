﻿using System.Collections.Generic;
using System.Linq;
using AutoFixture.Xunit2;
using FluentValidation.TestHelper;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ServiceLevelAgreements;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.ServiceLevelAgreementValidators;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators
{
    public static class DeleteServiceLevelModelValidatorTests
    {
        [Theory]
        [CommonAutoData]
        public static void Validate_PublishedSolutionWithManyServiceLevels_NoModelError(
               Solution solution,
               ServiceLevelAgreements serviceLevelAgreement,
               List<SlaServiceLevel> serviceLevels,
               [Frozen] Mock<ISolutionsService> solutionsService,
               DeleteServiceLevelModelValidator validator)
        {
            var serviceLevel = serviceLevels.First();
            serviceLevelAgreement.ServiceLevels = serviceLevels;
            solution.ServiceLevelAgreement = serviceLevelAgreement;
            solution.CatalogueItem.PublishedStatus = PublicationStatus.Published;

            var model = new DeleteServiceLevelModel(solution.CatalogueItem, serviceLevel);

            solutionsService.Setup(s => s.GetSolutionThin(solution.CatalogueItemId))
                .ReturnsAsync(solution.CatalogueItem);

            var result = validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(m => m);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_PublishedSolutionWitSingleServiceLevels_SetsModelError(
            Solution solution,
            ServiceLevelAgreements serviceLevelAgreement,
            SlaServiceLevel serviceLevel,
            [Frozen] Mock<ISolutionsService> solutionsService,
            DeleteServiceLevelModelValidator validator)
        {
            serviceLevelAgreement.ServiceLevels.Clear();
            serviceLevelAgreement.ServiceLevels.Add(serviceLevel);
            solution.ServiceLevelAgreement = serviceLevelAgreement;
            solution.CatalogueItem.PublishedStatus = PublicationStatus.Published;

            var model = new DeleteServiceLevelModel(solution.CatalogueItem, serviceLevel);

            solutionsService.Setup(s => s.GetSolutionWithServiceLevelAgreements(solution.CatalogueItemId))
                .ReturnsAsync(solution.CatalogueItem);

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m)
                .WithErrorMessage("This is the only service level provided and can only be deleted if you unpublish your solution first");
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_UnpublishedSolutionWithManyServiceAgreementTimes_NoModelError(
            Solution solution,
            ServiceLevelAgreements serviceLevelAgreement,
            List<SlaServiceLevel> serviceLevels,
            [Frozen] Mock<ISolutionsService> solutionsService,
            DeleteServiceLevelModelValidator validator)
        {
            var serviceLevel = serviceLevels.First();
            serviceLevelAgreement.ServiceLevels = serviceLevels;
            solution.ServiceLevelAgreement = serviceLevelAgreement;
            solution.CatalogueItem.PublishedStatus = PublicationStatus.Unpublished;

            var model = new DeleteServiceLevelModel(solution.CatalogueItem, serviceLevel);

            solutionsService.Setup(s => s.GetSolutionThin(solution.CatalogueItemId))
                .ReturnsAsync(solution.CatalogueItem);

            var result = validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(m => m);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_UnpublishedSolutionWitSingleServiceAgreementTimes_NoModelError(
            Solution solution,
            ServiceLevelAgreements serviceLevelAgreement,
            SlaServiceLevel serviceLevel,
            [Frozen] Mock<ISolutionsService> solutionsService,
            DeleteServiceLevelModelValidator validator)
        {
            serviceLevelAgreement.ServiceLevels.Clear();
            serviceLevelAgreement.ServiceLevels.Add(serviceLevel);
            solution.ServiceLevelAgreement = serviceLevelAgreement;
            solution.CatalogueItem.PublishedStatus = PublicationStatus.Unpublished;

            var model = new DeleteServiceLevelModel(solution.CatalogueItem, serviceLevel);

            solutionsService.Setup(s => s.GetSolutionThin(solution.CatalogueItemId))
                .ReturnsAsync(solution.CatalogueItem);

            var result = validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(m => m);
        }
    }
}
