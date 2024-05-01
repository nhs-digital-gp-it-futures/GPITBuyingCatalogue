:r ./InsertOrganisationTypes.sql
:r ./InsertRoles.sql

:r ./OdsOrganisationsSeedData/InsertOdsOrganisations.sql
:r ./OdsOrganisationsSeedData/InsertRoleTypes.sql
:r ./OdsOrganisationsSeedData/InsertRelationshipTypes.sql
:r ./OdsOrganisationsSeedData/InsertOrganisationRoles.sql
:r ./OdsOrganisationsSeedData/InsertOrganisationRelationships.sql

:r ./CreateExecutiveAgency.sql
:r ./CreateExecutiveAgencyUser.sql
:r ./CreateIntegratedCareBoards.sql
:r ./CreateCommissioningSupportUnits.sql
:r ./CreateTestUsers.sql

:r ./InsertAllowedEmailDomains.sql
:r ./InsertCapabilityStatuses.sql
:r ./TestData/InsertCapabilityCategories.sql
:r ./InsertCompliancyLevels.sql
:r ./InsertPublicationStatuses.sql
:r ./InsertSolutionCapabilityStatuses.sql
:r ./InsertSolutionEpicStatuses.sql
:r ./InsertCatalogueItemTypes.sql
:r ./TestData/InsertFrameworks.sql
:r ./TestData/InsertCapabilities.sql
:r ./InsertCataloguePriceTypes.sql
:r ./InsertCataloguePriceCalculationTypes.sql
:r ./InsertCataloguePriceQuantityCalculationTypes.sql
:r ./TestData/InsertEpics.sql
:r ./InsertPricingUnits.sql
:r ./InsertProvisioningTypes.sql
:r ./InsertSuppliers.sql
:r ./InsertStandardTypes.sql
:r ./TestData/InsertStandards.sql
:r ./TestData/InsertStandardsCapabilities.sql

:r ./InsertOrderStatuses.sql
:r ./InsertOrderTriageValues.sql
:r ./InsertOrderItemFundingTypes.sql
:r ./InsertTimeUnits.sql
:r ./InsertSolutions.sql
:r ./InsertAdditionalServices.sql
:r ./InsertAssociatedServices.sql
:r ./InsertRelatedOrganisations.sql

:r ./InsertEmailPreferenceTypes.sql
:r ./InsertEventTypes.sql
:r ./InsertEmailNotificationType.sql

:r ./DropImport.sql
:r ./DropPublish.sql

:r ./ProdLikeData/MergeSuppliers.sql
:r ./ProdLikeData/MergeCatalogueItems.sql
:r ./ProdLikeData/MergeSolutions.sql
:r ./ProdLikeData/MergeAdditionalServices.sql
:r ./ProdLikeData/MergeAssociatedServices.sql
:r ./ProdLikeData/MergeMarketingContacts.sql
:r ./ProdLikeData/MergeCatalogueItemEpics.sql
:r ./ProdLikeData/MergeCatalogueItemCapabilities.sql
:r ./ProdLikeData/MergeFrameworkSolutions.sql
:r ./ProdLikeData/MergeCataloguePrices.sql

:r ./OrderSeedData/InsertTestOrderSeedData.sql
:r ./InsertSupplierServiceAssociations.sql
:r ./InsertDefaultImplementationPlan.sql

:r ./TestData/InsertFilters.sql
:r ./TestData/Competitions/InsertCompetitions.sql
:r ./TestData/Solutions/InsertServiceLevels.sql

:r ./MigrateEpics.sql
:r ./MigrateEpicCompliancyLevel.sql
:r ./MigrateContracts.sql
:r ./MigrateOrderItemRecipients.sql
:r ./MigrateMergersAndSplits.sql
:r ./MigrateOrderType.sql
:r ./MigrateFrameworks.sql
