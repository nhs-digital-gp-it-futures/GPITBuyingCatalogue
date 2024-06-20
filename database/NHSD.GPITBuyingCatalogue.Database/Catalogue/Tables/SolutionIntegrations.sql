CREATE TABLE [catalogue].[SolutionIntegrations]
(
    [Id] INT IDENTITY (1, 1) NOT NULL PRIMARY KEY,
    [CatalogueItemId] NVARCHAR(14) NOT NULL,
    [IntegrationTypeId] INT NOT NULL,
    [Description] NVARCHAR(1000) NULL,
    [IsConsumer] BIT DEFAULT 0,
    [IntegratesWith] NVARCHAR(100) NULL,
    CONSTRAINT FK_SolutionIntegrations_Solution FOREIGN KEY ([CatalogueItemId]) REFERENCES catalogue.Solutions ([CatalogueItemId]) ON DELETE CASCADE,
    CONSTRAINT FK_SolutionIntegrations_IntegrationType FOREIGN KEY ([IntegrationTypeId]) REFERENCES catalogue.IntegrationTypes ([Id]) ON DELETE CASCADE
)
