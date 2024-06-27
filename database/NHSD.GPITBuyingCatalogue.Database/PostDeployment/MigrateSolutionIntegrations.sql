-- Migrate Solution Integrations to new Integrations tables
IF NOT EXISTS(SELECT 1 FROM catalogue.SolutionIntegrations)
BEGIN
    INSERT INTO catalogue.SolutionIntegrations ([CatalogueItemId], [IntegrationTypeId], [Description], [IsConsumer], [IntegratesWith])
    SELECT  [CatalogueItemId],
            IT.[Id],
            ISNULL(NULLIF(IntegrationData.[Description], ''), IntegrationData.[AdditionalInformation]),
            IntegrationData.[IsConsumer],
            ISNULL(NULLIF(IntegrationData.[IntegratesWith], ''), NULL)
    FROM catalogue.Solutions CROSS APPLY OPENJSON(Integrations) WITH (
	    [Id] NVARCHAR(2000) '$.Id',
	    [AdditionalInformation] NVARCHAR(500) '$.AdditionalInformation',
	    [Description] NVARCHAR(2000) '$.Description',
	    [IsConsumer] BIT '$.IsConsumer',
	    [IntegratesWith] NVARCHAR(200) '$.IntegratesWith',
	    [IntegrationType] NVARCHAR(15) '$.IntegrationType',
	    [Qualifier] NVARCHAR(100) '$.Qualifier'
    ) AS IntegrationData
    INNER JOIN catalogue.IntegrationTypes IT ON IntegrationData.Qualifier = IT.[Name]
END;

-- Migrate Competition Interop Criteria
IF NOT EXISTS(SELECT 1 FROM competitions.IntegrationsCriteria)
BEGIN
    INSERT INTO competitions.IntegrationsCriteria ([NonPriceElementsId], [IntegrationTypeId])
    SELECT  NonPriceElementsId,
            IT.[Id]
    FROM competitions.InteroperabilityCriteria
    INNER JOIN catalogue.IntegrationTypes IT ON Qualifier = IT.[Name]
END;

-- Migrate Filtering Integrations
IF NOT EXISTS(SELECT 1 FROM filtering.FilterIntegrations)
BEGIN
    INSERT INTO filtering.FilterIntegrations ([FilterId], [IntegrationId])
    SELECT FilterId, I.[Id]
    FROM filtering.FilterInteroperabilityIntegrationTypes
    INNER JOIN catalogue.Integrations I ON InteroperabilityIntegrationTypeId = I.[Id]
END;

-- Migrate all Integration Types
IF NOT EXISTS(SELECT 1 FROM filtering.FilterIntegrationTypes)
BEGIN
    INSERT INTO filtering.FilterIntegrationTypes ([FilterId], [IntegrationId], [IntegrationTypeId])
    SELECT FilterId, IT.[IntegrationId], IT.[Id]
    FROM filtering.FilterIM1IntegrationTypes
    INNER JOIN catalogue.IntegrationTypes IT ON IT.[Name] = CASE
        WHEN IM1IntegrationsTypeId = 0 THEN 'Bulk'
        WHEN IM1IntegrationsTypeId = 1 THEN 'Transactional'
        ELSE 'Patient Facing'
    END

    INSERT INTO filtering.FilterIntegrationTypes ([FilterId], [IntegrationId], [IntegrationTypeId])
    SELECT FilterId, IT.[IntegrationId], IT.[Id]
    FROM filtering.FilterGPConnectIntegrationTypes
    INNER JOIN catalogue.IntegrationTypes IT ON IT.[Name] = CASE
        WHEN IM1IntegrationsTypeId = 0 THEN 'Access Record HTML'
        WHEN IM1IntegrationsTypeId = 1 THEN 'Appointment Management'
        WHEN IM1IntegrationsTypeId = 2 THEN 'Access Record Structured'
        WHEN IM1IntegrationsTypeId = 3 THEN 'Send Document'
        ELSE 'Update Record'
    END

    INSERT INTO filtering.FilterIntegrationTypes ([FilterId], [IntegrationId], [IntegrationTypeId])
    SELECT FilterId, IT.[IntegrationId], IT.[Id]
    FROM filtering.FilterNhsAppIntegrationTypes
    INNER JOIN catalogue.IntegrationTypes IT ON IT.[Name] = CASE
        WHEN IM1IntegrationsTypeId = 0 THEN 'Online Consultations'
        WHEN IM1IntegrationsTypeId = 1 THEN 'Personal Health Records'
        WHEN IM1IntegrationsTypeId = 2 THEN 'Primary Care Notifications'
        ELSE 'Secondary Care Notifications'
    END

END;
