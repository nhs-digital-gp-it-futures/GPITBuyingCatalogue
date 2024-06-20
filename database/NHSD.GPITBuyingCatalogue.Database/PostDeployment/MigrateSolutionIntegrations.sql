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
