CREATE TABLE [filtering].[FilterIntegrations]
(
    [FilterId] INT NOT NULL,
    [IntegrationId] INT NOT NULL,
    CONSTRAINT PK_FilterIntegrations PRIMARY KEY ([FilterId], [IntegrationId]),
    CONSTRAINT FK_FilterIntegrations_Integration FOREIGN KEY ([IntegrationId]) REFERENCES catalogue.Integrations ([Id]) ON DELETE CASCADE,
    CONSTRAINT FK_FilterIntegrations_Filter FOREIGN KEY ([FilterId]) REFERENCES filtering.Filters ([Id]) ON DELETE CASCADE
)
