:r ./InfrastructureUsers.sql
:r ./InsertRoles.sql

-- Lookup data
:r ./Seed/InsertSeedIntegrations.sql
:r ./InsertAllowedEmailDomains.sql
:r ./InsertCapabilityStatuses.sql
:r ./InsertCompliancyLevels.sql
:r ./InsertPublicationStatuses.sql
:r ./InsertSolutionCapabilityStatuses.sql
:r ./InsertSolutionEpicStatuses.sql
:r ./InsertCatalogueItemTypes.sql
:r ./InsertCataloguePriceTypes.sql
:r ./InsertCataloguePriceCalculationTypes.sql
:r ./InsertCataloguePriceQuantityCalculationTypes.sql
:r ./InsertPricingUnits.sql
:r ./InsertProvisioningTypes.sql
:r ./InsertStandardTypes.sql
:r ./InsertOrderTriageValues.sql
:r ./InsertOrderItemFundingTypes.sql
:r ./InsertTimeUnits.sql
:r ./InsertEmailPreferenceRoleTypes.sql
:r ./InsertEmailPreferenceTypes.sql
:r ./InsertEventTypes.sql
:r ./InsertEmailNotificationType.sql
:r ./InsertDefaultImplementationPlan.sql
:r ./InsertOrganisationTypes.sql
:r ./InsertSolutionStandardStatuses.sql

-- Organisation Data
:r ./CreateExecutiveAgency.sql
:r ./CreateExecutiveAgencyUser.sql
:r ./CreateCommissioningSupportUnits.sql

-----------------------------------------------------------------
--                          TEST DATA                          --
-----------------------------------------------------------------

-- Insert ODS Organisation Data
:r ./TestData/OdsOrganisationsSeedData/InsertOdsOrganisations.sql
:r ./TestData/OdsOrganisationsSeedData/InsertRoleTypes.sql
:r ./TestData/OdsOrganisationsSeedData/InsertRelationshipTypes.sql
:r ./TestData/OdsOrganisationsSeedData/InsertOrganisationRoles.sql
:r ./TestData/OdsOrganisationsSeedData/InsertOrganisationRelationships.sql

-- Insert Organisation Data
:r ./TestData/CreateIntegratedCareBoards.sql
:r ./TestData/InsertRelatedOrganisations.sql

-- Insert Test Users
:r ./TestData/CreateTestUsers.sql

-- Insert lookup test data (required for Solutions)
:r ./TestData/InsertFrameworks.sql
:r ./TestData/InsertStandards.sql
:r ./TestData/InsertCapabilityCategories.sql
:r ./TestData/InsertCapabilities.sql
:r ./TestData/InsertStandardsCapabilities.sql
:r ./TestData/InsertEpics.sql

-- Insert Solutions and Catalogue Items
:r ./TestData/Solutions/MergeSuppliers.sql
:r ./TestData/Solutions/MergeCatalogueItems.sql
:r ./TestData/Solutions/MergeSolutions.sql
:r ./TestData/Solutions/MergeAssociatedServices.sql
:r ./TestData/Solutions/MergeAdditionalServices.sql
:r ./TestData/Solutions/MergeCatalogueItemCapabilities.sql
:r ./TestData/Solutions/MergeCatalogueItemEpics.sql
:r ./TestData/Solutions/MergeCataloguePrices.sql
:r ./TestData/Solutions/MergeCataloguePriceTiers.sql
:r ./TestData/Solutions/MergeServiceLevelAgreements.sql
:r ./TestData/Solutions/MergeServiceAvailabilityTimes.sql
:r ./TestData/Solutions/MergeServiceLevelContacts.sql
:r ./TestData/Solutions/MergeServiceLevels.sql

-- Insert Orders, Filters and Competitions
:r ./TestData/InsertTestOrderSeedData.sql
:r ./TestData/InsertFilters.sql
:r ./TestData/Competitions/InsertCompetitions.sql

-----------------------------------------------------------------
--                      MIGRATION SCRIPTS                      --
-----------------------------------------------------------------

:r ./InsertUserLoginEvents.sql