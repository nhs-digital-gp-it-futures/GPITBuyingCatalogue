CREATE TABLE [competitions].[InteroperabilityCriteria]
(
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [NonPriceElementsId] INT NOT NULL,
    [Qualifier] NVARCHAR(40) NOT NULL,
    [IntegrationType] INT NOT NULL,
    CONSTRAINT FK_InteroperabilityCriteria_NonPriceElements FOREIGN KEY ([NonPriceElementsId]) REFERENCES [competitions].[NonPriceElements] ([Id]) ON DELETE CASCADE
)
