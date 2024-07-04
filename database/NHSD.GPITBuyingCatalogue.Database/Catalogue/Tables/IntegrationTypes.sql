CREATE TABLE [catalogue].[IntegrationTypes]
(
    [Id] INT IDENTITY(1, 1) NOT NULL PRIMARY KEY,
    [IntegrationId] INT NOT NULL,
    [Name] NVARCHAR(50) NOT NULL,
    [Description] NVARCHAR(350) NULL,
    CONSTRAINT FK_IntegrationTypes_Integration FOREIGN KEY ([IntegrationId]) REFERENCES catalogue.Integrations ([Id])
)
