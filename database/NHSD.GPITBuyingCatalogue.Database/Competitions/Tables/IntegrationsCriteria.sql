CREATE TABLE [competitions].[IntegrationsCriteria]
(
	[NonPriceElementsId] INT NOT NULL,
    [IntegrationTypeId] INT NOT NULL,
    CONSTRAINT PK_IntegrationsCriteria PRIMARY KEY ([NonPriceElementsId], [IntegrationTypeId]),
    CONSTRAINT FK_IntegrationsCriteria_NonPriceElements FOREIGN KEY ([NonPriceElementsId]) REFERENCES [competitions].[NonPriceElements] ([Id]) ON DELETE CASCADE,
    CONSTRAINT FK_IntegrationsCriteria_IntegrationType FOREIGN KEY ([IntegrationTypeId]) REFERENCES [catalogue].[IntegrationTypes] ([Id]) ON DELETE CASCADE
)
