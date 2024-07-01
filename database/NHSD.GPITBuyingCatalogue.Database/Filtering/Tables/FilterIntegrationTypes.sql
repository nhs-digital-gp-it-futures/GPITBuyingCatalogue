CREATE TABLE [filtering].[FilterIntegrationTypes]
(
    [FilterId] INT NOT NULL,
    [IntegrationId] INT NOT NULL,
    [IntegrationTypeId] INT NOT NULL,
    CONSTRAINT PK_FilterIntegrationTypes PRIMARY KEY ([FilterId], [IntegrationId], [IntegrationTypeId]),
    CONSTRAINT FK_FilterIntegrationTypes_Filter FOREIGN KEY ([FilterId]) REFERENCES filtering.Filters ([Id]),
    CONSTRAINT FK_FilterIntegrationTypes_Integration FOREIGN KEY ([FilterId], [IntegrationId]) REFERENCES filtering.FilterIntegrations ([FilterId], [IntegrationId]),
    CONSTRAINT FK_FilterIntegrationTypes_IntegrationType FOREIGN KEY ([IntegrationTypeId]) REFERENCES catalogue.IntegrationTypes ([Id]),
)
