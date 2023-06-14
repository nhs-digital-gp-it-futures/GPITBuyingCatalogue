using System.Collections.Generic;
using System.Linq;
using AutoFixture.Xunit2;
using FluentValidation.TestHelper;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions.Admin;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.CatalogueSolutionsModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators
{
    public static class ManageCatalogueSolutionModelValidatorTests
    {
        [Theory]
        [CommonAutoData]
        public static void Validate_SamePublicationStatus_NoModelError(
            Solution solution,
            [Frozen] Mock<ISolutionsService> solutionsService,
            ManageCatalogueSolutionModelValidator validator)
        {
            solution.CatalogueItem.PublishedStatus = PublicationStatus.Published;
            solutionsService.Setup(s => s.GetSolutionThin(solution.CatalogueItemId))
                .ReturnsAsync(solution.CatalogueItem);

            var model = new ManageCatalogueSolutionModel
            {
                SolutionId = solution.CatalogueItemId,
                SelectedPublicationStatus = PublicationStatus.Published,
            };

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_NonPublishedStatus_NoModelError(
            ManageCatalogueSolutionModelValidator validator)
        {
            var model = new ManageCatalogueSolutionModel
            {
                SelectedPublicationStatus = PublicationStatus.Unpublished,
            };

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_IncompleteDescription_SetsModelError(
            Solution solution,
            List<ServiceAvailabilityTimes> serviceAvailabilityTimes,
            List<SlaContact> slaContacts,
            List<SlaServiceLevel> slaServiceLevels,
            [Frozen] Mock<ISolutionsService> solutionsService,
            ManageCatalogueSolutionModelValidator validator)
        {
            solution.FullDescription = null;
            solution.AboutUrl = null;
            solution.Summary = null;
            solution.ServiceLevelAgreement.ServiceHours = serviceAvailabilityTimes;
            solution.ServiceLevelAgreement.ServiceLevels = slaServiceLevels;
            solution.ServiceLevelAgreement.Contacts = slaContacts;
            solution.CatalogueItem.CatalogueItemContacts.Add(solution.CatalogueItem.Supplier.SupplierContacts.First());
            solution.CatalogueItem.PublishedStatus = PublicationStatus.Draft;
            solutionsService.Setup(s => s.GetSolutionThin(solution.CatalogueItemId))
                .ReturnsAsync(solution.CatalogueItem);

            var model = new ManageCatalogueSolutionModel
            {
                SolutionId = solution.CatalogueItemId,
                SelectedPublicationStatus = PublicationStatus.Published,
            };

            var result = validator.TestValidate(model);

            result.ShouldHaveAnyValidationError()
                .WithErrorMessage("Complete all mandatory sections before publishing");
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_IncompleteApplicationType_SetsModelError(
            Solution solution,
            List<ServiceAvailabilityTimes> serviceAvailabilityTimes,
            List<SlaContact> slaContacts,
            List<SlaServiceLevel> slaServiceLevels,
            [Frozen] Mock<ISolutionsService> solutionsService,
            ManageCatalogueSolutionModelValidator validator)
        {
            solution.ClientApplication = string.Empty;
            solution.ServiceLevelAgreement.ServiceHours = serviceAvailabilityTimes;
            solution.ServiceLevelAgreement.ServiceLevels = slaServiceLevels;
            solution.ServiceLevelAgreement.Contacts = slaContacts;
            solution.CatalogueItem.CatalogueItemContacts.Add(solution.CatalogueItem.Supplier.SupplierContacts.First());
            solution.CatalogueItem.PublishedStatus = PublicationStatus.Draft;
            solutionsService.Setup(s => s.GetSolutionThin(solution.CatalogueItemId))
                .ReturnsAsync(solution.CatalogueItem);

            var model = new ManageCatalogueSolutionModel
            {
                SolutionId = solution.CatalogueItemId,
                SelectedPublicationStatus = PublicationStatus.Published,
            };

            var result = validator.TestValidate(model);

            result.ShouldHaveAnyValidationError()
                .WithErrorMessage("Complete all mandatory sections before publishing");
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_IncompleteHostingType_SetsModelError(
            Solution solution,
            List<ServiceAvailabilityTimes> serviceAvailabilityTimes,
            List<SlaContact> slaContacts,
            List<SlaServiceLevel> slaServiceLevels,
            [Frozen] Mock<ISolutionsService> solutionsService,
            ManageCatalogueSolutionModelValidator validator)
        {
            solution.Hosting.PublicCloud.Summary = string.Empty;
            solution.Hosting.PrivateCloud.Summary = string.Empty;
            solution.Hosting.HybridHostingType.Summary = string.Empty;
            solution.Hosting.OnPremise.Summary = string.Empty;

            solution.ServiceLevelAgreement.ServiceHours = serviceAvailabilityTimes;
            solution.ServiceLevelAgreement.ServiceLevels = slaServiceLevels;
            solution.ServiceLevelAgreement.Contacts = slaContacts;
            solution.CatalogueItem.CatalogueItemContacts.Add(solution.CatalogueItem.Supplier.SupplierContacts.First());
            solution.CatalogueItem.PublishedStatus = PublicationStatus.Draft;
            solutionsService.Setup(s => s.GetSolutionThin(solution.CatalogueItemId))
                .ReturnsAsync(solution.CatalogueItem);

            var model = new ManageCatalogueSolutionModel
            {
                SolutionId = solution.CatalogueItemId,
                SelectedPublicationStatus = PublicationStatus.Published,
            };

            var result = validator.TestValidate(model);

            result.ShouldHaveAnyValidationError()
                .WithErrorMessage("Complete all mandatory sections before publishing");
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_IncompleteListPrices_SetsModelError(
            Solution solution,
            List<ServiceAvailabilityTimes> serviceAvailabilityTimes,
            List<SlaContact> slaContacts,
            List<SlaServiceLevel> slaServiceLevels,
            [Frozen] Mock<ISolutionsService> solutionsService,
            ManageCatalogueSolutionModelValidator validator)
        {
            solution.CatalogueItem.CataloguePrices = new List<CataloguePrice>();

            solution.ServiceLevelAgreement.ServiceHours = serviceAvailabilityTimes;
            solution.ServiceLevelAgreement.ServiceLevels = slaServiceLevels;
            solution.ServiceLevelAgreement.Contacts = slaContacts;
            solution.CatalogueItem.CatalogueItemContacts.Add(solution.CatalogueItem.Supplier.SupplierContacts.First());
            solution.CatalogueItem.PublishedStatus = PublicationStatus.Draft;
            solutionsService.Setup(s => s.GetSolutionThin(solution.CatalogueItemId))
                .ReturnsAsync(solution.CatalogueItem);

            var model = new ManageCatalogueSolutionModel
            {
                SolutionId = solution.CatalogueItemId,
                SelectedPublicationStatus = PublicationStatus.Published,
            };

            var result = validator.TestValidate(model);

            result.ShouldHaveAnyValidationError()
                .WithErrorMessage("Complete all mandatory sections before publishing");
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_IncompleteSupplierDetails_SetsModelError(
            Solution solution,
            List<ServiceAvailabilityTimes> serviceAvailabilityTimes,
            List<SlaContact> slaContacts,
            List<SlaServiceLevel> slaServiceLevels,
            [Frozen] Mock<ISolutionsService> solutionsService,
            ManageCatalogueSolutionModelValidator validator)
        {
            solution.CatalogueItem.CatalogueItemContacts = new List<SupplierContact>();

            solution.ServiceLevelAgreement.ServiceHours = serviceAvailabilityTimes;
            solution.ServiceLevelAgreement.ServiceLevels = slaServiceLevels;
            solution.ServiceLevelAgreement.Contacts = slaContacts;
            solution.CatalogueItem.PublishedStatus = PublicationStatus.Draft;
            solutionsService.Setup(s => s.GetSolutionThin(solution.CatalogueItemId))
                .ReturnsAsync(solution.CatalogueItem);

            var model = new ManageCatalogueSolutionModel
            {
                SolutionId = solution.CatalogueItemId,
                SelectedPublicationStatus = PublicationStatus.Published,
            };

            var result = validator.TestValidate(model);

            result.ShouldHaveAnyValidationError()
                .WithErrorMessage("Complete all mandatory sections before publishing");
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_IncompleteCapabilities_SetsModelError(
            Solution solution,
            List<ServiceAvailabilityTimes> serviceAvailabilityTimes,
            List<SlaContact> slaContacts,
            List<SlaServiceLevel> slaServiceLevels,
            [Frozen] Mock<ISolutionsService> solutionsService,
            ManageCatalogueSolutionModelValidator validator)
        {
            solution.CatalogueItem.CatalogueItemCapabilities = new List<CatalogueItemCapability>();

            solution.ServiceLevelAgreement.ServiceHours = serviceAvailabilityTimes;
            solution.ServiceLevelAgreement.ServiceLevels = slaServiceLevels;
            solution.ServiceLevelAgreement.Contacts = slaContacts;
            solution.CatalogueItem.CatalogueItemContacts.Add(solution.CatalogueItem.Supplier.SupplierContacts.First());
            solution.CatalogueItem.PublishedStatus = PublicationStatus.Draft;
            solutionsService.Setup(s => s.GetSolutionThin(solution.CatalogueItemId))
                .ReturnsAsync(solution.CatalogueItem);

            var model = new ManageCatalogueSolutionModel
            {
                SolutionId = solution.CatalogueItemId,
                SelectedPublicationStatus = PublicationStatus.Published,
            };

            var result = validator.TestValidate(model);

            result.ShouldHaveAnyValidationError()
                .WithErrorMessage("Complete all mandatory sections before publishing");
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_IncompleteServiceLevelAgreement_SetsModelError(
            Solution solution,
            [Frozen] Mock<ISolutionsService> solutionsService,
            ManageCatalogueSolutionModelValidator validator)
        {
            solution.CatalogueItem.CatalogueItemContacts.Add(solution.CatalogueItem.Supplier.SupplierContacts.First());
            solution.CatalogueItem.PublishedStatus = PublicationStatus.Draft;
            solutionsService.Setup(s => s.GetSolutionThin(solution.CatalogueItemId))
                .ReturnsAsync(solution.CatalogueItem);

            var model = new ManageCatalogueSolutionModel
            {
                SolutionId = solution.CatalogueItemId,
                SelectedPublicationStatus = PublicationStatus.Published,
            };

            var result = validator.TestValidate(model);

            result.ShouldHaveAnyValidationError()
                .WithErrorMessage("Complete all mandatory sections before publishing");
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_Valid_NoModelError(
            Solution solution,
            [Frozen] Mock<ISolutionsService> solutionsService,
            ManageCatalogueSolutionModelValidator validator)
        {
            SolutionLoadingStatusesModel solutionLoadingStatuses = new SolutionLoadingStatusesModel
            {
                Description = ServiceContracts.Enums.TaskProgress.Completed,
                AdditionalServices = ServiceContracts.Enums.TaskProgress.Completed,
                AssociatedServices = ServiceContracts.Enums.TaskProgress.Completed,
                CapabilitiesAndEpics = ServiceContracts.Enums.TaskProgress.Completed,
                ApplicationType = ServiceContracts.Enums.TaskProgress.Completed,
                DevelopmentPlans = ServiceContracts.Enums.TaskProgress.Completed,
                Features = ServiceContracts.Enums.TaskProgress.Completed,
                HostingType = ServiceContracts.Enums.TaskProgress.Completed,
                Implementation = ServiceContracts.Enums.TaskProgress.Completed,
                Interoperability = ServiceContracts.Enums.TaskProgress.Completed,
                ListPrice = ServiceContracts.Enums.TaskProgress.Completed,
                ServiceLevelAgreement = ServiceContracts.Enums.TaskProgress.Completed,
                SupplierDetails = ServiceContracts.Enums.TaskProgress.Completed,
            };

            solution.CatalogueItem.PublishedStatus = PublicationStatus.Draft;
            solutionsService.Setup(s => s.GetSolutionThin(solution.CatalogueItemId))
                .ReturnsAsync(solution.CatalogueItem);

            var model = new ManageCatalogueSolutionModel(solutionLoadingStatuses, solution.CatalogueItem)
            {
                SelectedPublicationStatus = PublicationStatus.Published,
            };

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
