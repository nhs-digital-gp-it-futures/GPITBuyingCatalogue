using System.Collections.Generic;
using System.Linq;
using AutoFixture.Xunit2;
using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions.Admin;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.CatalogueSolutionsModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators
{
    public static class ManageCatalogueSolutionModelValidatorTests
    {
        private static readonly SolutionSections GPITSolutionSections = new()
        {
            SolutionType = SolutionType.GPIT,
            Details = SolutionSectionRequirement.Mandatory,
            Description = SolutionSectionRequirement.Mandatory,
            Features = SolutionSectionRequirement.Optional,
            Integrations = SolutionSectionRequirement.Optional,
            Implementation = SolutionSectionRequirement.Optional,
            ApplicationType = SolutionSectionRequirement.Mandatory,
            HostingType = SolutionSectionRequirement.Optional,
            DataProcessingInformation = SolutionSectionRequirement.Optional,
            ListPrice = SolutionSectionRequirement.Mandatory,
            CapabilitiesAndEpics = SolutionSectionRequirement.Mandatory,
            AdditionalServices = SolutionSectionRequirement.Optional,
            AssociatedServices = SolutionSectionRequirement.Optional,
            Standards = SolutionSectionRequirement.Optional,
            DevelopmentPlans = SolutionSectionRequirement.Optional,
            SupplierDetails = SolutionSectionRequirement.Mandatory,
            ServiceLevelAgreement = SolutionSectionRequirement.Mandatory,
        };

        private static readonly string[] MandatorySections =
        [
            "Description",
            "Features",
            "Integrations",
            "Implementation",
            "ApplicationType",
            "HostingType",
            "DataProcessingInformation",
            "ListPrice",
            "CapabilitiesAndEpics",
            "AdditionalServices",
            "AssociatedServices",
            "Standards",
            "DevelopmentPlans",
            "SupplierDetails",
            "ServiceLevelAgreement",
        ];

        public static IEnumerable<object[]> MandatorySectionCases =>
            MandatorySections.SelectMany(GetMandatorySectionCases);

        [Theory]
        [MockMemberAutoData(nameof(MandatorySectionCases))]
        public static void Validate_MandatorySectionStatus_ReturnsExpectedResult(
            string section,
            bool isCompleted,
            Solution solution,
            [Frozen] ISolutionsService solutionsService,
            ManageCatalogueSolutionModelValidator validator)
        {
            solution.CatalogueItem.PublishedStatus = PublicationStatus.Draft;
            solutionsService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var model = CreateModel(solution.CatalogueItemId, section, isCompleted ? TaskProgress.Completed : TaskProgress.NotStarted);

            var result = validator.TestValidate(model);

            if (isCompleted)
            {
                result.ShouldNotHaveAnyValidationErrors();
            }
            else
            {
                result.ShouldHaveAnyValidationError()
                    .WithErrorMessage("Complete all mandatory sections before publishing");
            }
        }

        [Theory]
        [MockAutoData]
        public static void Validate_SamePublicationStatus_NoModelError(
            Solution solution,
            [Frozen] ISolutionsService solutionsService,
            ManageCatalogueSolutionModelValidator validator)
        {
            solution.CatalogueItem.PublishedStatus = PublicationStatus.Published;
            solutionsService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var model = new ManageCatalogueSolutionModel
            {
                SolutionId = solution.CatalogueItemId,
                SelectedPublicationStatus = PublicationStatus.Published,
                SolutionType = SolutionType.GPIT,
            };

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [MockAutoData]
        public static void Validate_NonPublishedStatus_NoModelError(
            ManageCatalogueSolutionModelValidator validator)
        {
            var model = new ManageCatalogueSolutionModel
            {
                SelectedPublicationStatus = PublicationStatus.Unpublished,
                SolutionType = SolutionType.GPIT,
            };

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [MockAutoData]
        public static void Validate_IncompleteDescription_SetsModelError(
            Solution solution,
            List<ServiceAvailabilityTimes> serviceAvailabilityTimes,
            List<SlaContact> slaContacts,
            List<SlaServiceLevel> slaServiceLevels,
            [Frozen] ISolutionsService solutionsService,
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
            solutionsService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var model = new ManageCatalogueSolutionModel
            {
                SolutionId = solution.CatalogueItemId,
                SelectedPublicationStatus = PublicationStatus.Published,
                SolutionType = SolutionType.GPIT,
                SolutionSections = GPITSolutionSections,
            };

            var result = validator.TestValidate(model);

            result.ShouldHaveAnyValidationError()
                .WithErrorMessage("Complete all mandatory sections before publishing");
        }

        [Theory]
        [MockAutoData]
        public static void Validate_IncompleteApplicationType_SetsModelError(
            Solution solution,
            List<ServiceAvailabilityTimes> serviceAvailabilityTimes,
            List<SlaContact> slaContacts,
            List<SlaServiceLevel> slaServiceLevels,
            [Frozen] ISolutionsService solutionsService,
            ManageCatalogueSolutionModelValidator validator)
        {
            solution.ApplicationTypeDetail = null;
            solution.ServiceLevelAgreement.ServiceHours = serviceAvailabilityTimes;
            solution.ServiceLevelAgreement.ServiceLevels = slaServiceLevels;
            solution.ServiceLevelAgreement.Contacts = slaContacts;
            solution.CatalogueItem.CatalogueItemContacts.Add(solution.CatalogueItem.Supplier.SupplierContacts.First());
            solution.CatalogueItem.PublishedStatus = PublicationStatus.Draft;
            solutionsService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var model = new ManageCatalogueSolutionModel
            {
                SolutionId = solution.CatalogueItemId,
                SelectedPublicationStatus = PublicationStatus.Published,
                SolutionType = SolutionType.GPIT,
                SolutionSections = GPITSolutionSections,
            };

            var result = validator.TestValidate(model);

            result.ShouldHaveAnyValidationError()
                .WithErrorMessage("Complete all mandatory sections before publishing");
        }

        [Theory]
        [MockAutoData]
        public static void Validate_IncompleteHostingType_SetsModelError(
            Solution solution,
            List<ServiceAvailabilityTimes> serviceAvailabilityTimes,
            List<SlaContact> slaContacts,
            List<SlaServiceLevel> slaServiceLevels,
            [Frozen] ISolutionsService solutionsService,
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
            solutionsService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var model = new ManageCatalogueSolutionModel
            {
                SolutionId = solution.CatalogueItemId,
                SelectedPublicationStatus = PublicationStatus.Published,
                SolutionType = SolutionType.GPIT,
                SolutionSections = GPITSolutionSections,
            };

            var result = validator.TestValidate(model);

            result.ShouldHaveAnyValidationError()
                .WithErrorMessage("Complete all mandatory sections before publishing");
        }

        [Theory]
        [MockAutoData]
        public static void Validate_IncompleteListPrices_SetsModelError(
            Solution solution,
            List<ServiceAvailabilityTimes> serviceAvailabilityTimes,
            List<SlaContact> slaContacts,
            List<SlaServiceLevel> slaServiceLevels,
            [Frozen] ISolutionsService solutionsService,
            ManageCatalogueSolutionModelValidator validator)
        {
            solution.CatalogueItem.CataloguePrices = new List<CataloguePrice>();

            solution.ServiceLevelAgreement.ServiceHours = serviceAvailabilityTimes;
            solution.ServiceLevelAgreement.ServiceLevels = slaServiceLevels;
            solution.ServiceLevelAgreement.Contacts = slaContacts;
            solution.CatalogueItem.CatalogueItemContacts.Add(solution.CatalogueItem.Supplier.SupplierContacts.First());
            solution.CatalogueItem.PublishedStatus = PublicationStatus.Draft;
            solutionsService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var model = new ManageCatalogueSolutionModel
            {
                SolutionId = solution.CatalogueItemId,
                SelectedPublicationStatus = PublicationStatus.Published,
                SolutionType = SolutionType.GPIT,
                SolutionSections = GPITSolutionSections,
            };

            var result = validator.TestValidate(model);

            result.ShouldHaveAnyValidationError()
                .WithErrorMessage("Complete all mandatory sections before publishing");
        }

        [Theory]
        [MockAutoData]
        public static void Validate_IncompleteSupplierDetails_SetsModelError(
            Solution solution,
            List<ServiceAvailabilityTimes> serviceAvailabilityTimes,
            List<SlaContact> slaContacts,
            List<SlaServiceLevel> slaServiceLevels,
            [Frozen] ISolutionsService solutionsService,
            ManageCatalogueSolutionModelValidator validator)
        {
            solution.CatalogueItem.CatalogueItemContacts = new List<SupplierContact>();

            solution.ServiceLevelAgreement.ServiceHours = serviceAvailabilityTimes;
            solution.ServiceLevelAgreement.ServiceLevels = slaServiceLevels;
            solution.ServiceLevelAgreement.Contacts = slaContacts;
            solution.CatalogueItem.PublishedStatus = PublicationStatus.Draft;
            solutionsService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var model = new ManageCatalogueSolutionModel
            {
                SolutionId = solution.CatalogueItemId,
                SelectedPublicationStatus = PublicationStatus.Published,
                SolutionType = SolutionType.GPIT,
                SolutionSections = GPITSolutionSections,
            };

            var result = validator.TestValidate(model);

            result.ShouldHaveAnyValidationError()
                .WithErrorMessage("Complete all mandatory sections before publishing");
        }

        [Theory]
        [MockAutoData]
        public static void Validate_IncompleteCapabilities_SetsModelError(
            Solution solution,
            List<ServiceAvailabilityTimes> serviceAvailabilityTimes,
            List<SlaContact> slaContacts,
            List<SlaServiceLevel> slaServiceLevels,
            [Frozen] ISolutionsService solutionsService,
            ManageCatalogueSolutionModelValidator validator)
        {
            solution.CatalogueItem.CatalogueItemCapabilities = new List<CatalogueItemCapability>();

            solution.ServiceLevelAgreement.ServiceHours = serviceAvailabilityTimes;
            solution.ServiceLevelAgreement.ServiceLevels = slaServiceLevels;
            solution.ServiceLevelAgreement.Contacts = slaContacts;
            solution.CatalogueItem.CatalogueItemContacts.Add(solution.CatalogueItem.Supplier.SupplierContacts.First());
            solution.CatalogueItem.PublishedStatus = PublicationStatus.Draft;
            solutionsService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var model = new ManageCatalogueSolutionModel
            {
                SolutionId = solution.CatalogueItemId,
                SelectedPublicationStatus = PublicationStatus.Published,
                SolutionType = SolutionType.GPIT,
                SolutionSections = GPITSolutionSections,
            };

            var result = validator.TestValidate(model);

            result.ShouldHaveAnyValidationError()
                .WithErrorMessage("Complete all mandatory sections before publishing");
        }

        [Theory]
        [MockAutoData]
        public static void Validate_IncompleteServiceLevelAgreement_SetsModelError(
            Solution solution,
            [Frozen] ISolutionsService solutionsService,
            ManageCatalogueSolutionModelValidator validator)
        {
            solution.CatalogueItem.CatalogueItemContacts.Add(solution.CatalogueItem.Supplier.SupplierContacts.First());
            solution.CatalogueItem.PublishedStatus = PublicationStatus.Draft;
            solutionsService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var model = new ManageCatalogueSolutionModel
            {
                SolutionId = solution.CatalogueItemId,
                SelectedPublicationStatus = PublicationStatus.Published,
                SolutionType = SolutionType.GPIT,
                SolutionSections = GPITSolutionSections,
            };

            var result = validator.TestValidate(model);

            result.ShouldHaveAnyValidationError()
                .WithErrorMessage("Complete all mandatory sections before publishing");
        }

        [Theory]
        [MockAutoData]
        public static void Validate_Valid_NoModelError(
            Solution solution,
            [Frozen] ISolutionsService solutionsService,
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
            solutionsService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var model = new ManageCatalogueSolutionModel(solutionLoadingStatuses, solution.CatalogueItem, new SolutionSections())
            {
                SelectedPublicationStatus = PublicationStatus.Published,
                SolutionType = SolutionType.GPIT,
            };

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }

        private static IEnumerable<object[]> GetMandatorySectionCases(string section)
        {
            yield return new object[] { section, false };
            yield return new object[] { section, true };
        }

        private static ManageCatalogueSolutionModel CreateModel(
            CatalogueItemId solutionId,
            string section,
            TaskProgress status)
        {
            var sections = new SolutionSections();

            switch (section)
            {
                case "Description":
                    sections.Description = SolutionSectionRequirement.Mandatory;
                    return new ManageCatalogueSolutionModel { SolutionId = solutionId, SelectedPublicationStatus = PublicationStatus.Published, SolutionType = SolutionType.GPIT, SolutionSections = sections, DescriptionStatus = status };
                case "Features":
                    sections.Features = SolutionSectionRequirement.Mandatory;
                    return new ManageCatalogueSolutionModel { SolutionId = solutionId, SelectedPublicationStatus = PublicationStatus.Published, SolutionType = SolutionType.GPIT, SolutionSections = sections, FeaturesStatus = status };
                case "Integrations":
                    sections.Integrations = SolutionSectionRequirement.Mandatory;
                    return new ManageCatalogueSolutionModel { SolutionId = solutionId, SelectedPublicationStatus = PublicationStatus.Published, SolutionType = SolutionType.GPIT, SolutionSections = sections, InteroperabilityStatus = status };
                case "Implementation":
                    sections.Implementation = SolutionSectionRequirement.Mandatory;
                    return new ManageCatalogueSolutionModel { SolutionId = solutionId, SelectedPublicationStatus = PublicationStatus.Published, SolutionType = SolutionType.GPIT, SolutionSections = sections, ImplementationStatus = status };
                case "ApplicationType":
                    sections.ApplicationType = SolutionSectionRequirement.Mandatory;
                    return new ManageCatalogueSolutionModel { SolutionId = solutionId, SelectedPublicationStatus = PublicationStatus.Published, SolutionType = SolutionType.GPIT, SolutionSections = sections, ApplicationTypeStatus = status };
                case "HostingType":
                    sections.HostingType = SolutionSectionRequirement.Mandatory;
                    return new ManageCatalogueSolutionModel { SolutionId = solutionId, SelectedPublicationStatus = PublicationStatus.Published, SolutionType = SolutionType.GPIT, SolutionSections = sections, HostingTypeStatus = status };
                case "DataProcessingInformation":
                    sections.DataProcessingInformation = SolutionSectionRequirement.Mandatory;
                    return new ManageCatalogueSolutionModel { SolutionId = solutionId, SelectedPublicationStatus = PublicationStatus.Published, SolutionType = SolutionType.GPIT, SolutionSections = sections, DataProcessingStatus = status };
                case "ListPrice":
                    sections.ListPrice = SolutionSectionRequirement.Mandatory;
                    return new ManageCatalogueSolutionModel { SolutionId = solutionId, SelectedPublicationStatus = PublicationStatus.Published, SolutionType = SolutionType.GPIT, SolutionSections = sections, ListPriceStatus = status };
                case "CapabilitiesAndEpics":
                    sections.CapabilitiesAndEpics = SolutionSectionRequirement.Mandatory;
                    return new ManageCatalogueSolutionModel { SolutionId = solutionId, SelectedPublicationStatus = PublicationStatus.Published, SolutionType = SolutionType.GPIT, SolutionSections = sections, CapabilitiesStatus = status };
                case "AdditionalServices":
                    sections.AdditionalServices = SolutionSectionRequirement.Mandatory;
                    return new ManageCatalogueSolutionModel { SolutionId = solutionId, SelectedPublicationStatus = PublicationStatus.Published, SolutionType = SolutionType.GPIT, SolutionSections = sections, AdditionalServicesStatus = status };
                case "AssociatedServices":
                    sections.AssociatedServices = SolutionSectionRequirement.Mandatory;
                    return new ManageCatalogueSolutionModel { SolutionId = solutionId, SelectedPublicationStatus = PublicationStatus.Published, SolutionType = SolutionType.GPIT, SolutionSections = sections, AssociatedServicesStatus = status };
                case "Standards":
                    sections.Standards = SolutionSectionRequirement.Mandatory;
                    return new ManageCatalogueSolutionModel { SolutionId = solutionId, SelectedPublicationStatus = PublicationStatus.Published, SolutionType = SolutionType.GPIT, SolutionSections = sections, StandardsStatus = status };
                case "DevelopmentPlans":
                    sections.DevelopmentPlans = SolutionSectionRequirement.Mandatory;
                    return new ManageCatalogueSolutionModel { SolutionId = solutionId, SelectedPublicationStatus = PublicationStatus.Published, SolutionType = SolutionType.GPIT, SolutionSections = sections, RoadmapStatus = status };
                case "SupplierDetails":
                    sections.SupplierDetails = SolutionSectionRequirement.Mandatory;
                    return new ManageCatalogueSolutionModel { SolutionId = solutionId, SelectedPublicationStatus = PublicationStatus.Published, SolutionType = SolutionType.GPIT, SolutionSections = sections, SupplierDetailsStatus = status };
                case "ServiceLevelAgreement":
                    sections.ServiceLevelAgreement = SolutionSectionRequirement.Mandatory;
                    return new ManageCatalogueSolutionModel { SolutionId = solutionId, SelectedPublicationStatus = PublicationStatus.Published, SolutionType = SolutionType.GPIT, SolutionSections = sections, SlaStatus = status };
            }

            return new ManageCatalogueSolutionModel { SolutionId = solutionId, SelectedPublicationStatus = PublicationStatus.Published, SolutionType = SolutionType.GPIT, SolutionSections = sections };
        }
    }
}
