CREATE TABLE [competitions].[ImplementationCriteria]
(
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [NonPriceElementsId] INT NOT NULL,
    [Requirements] NVARCHAR(1100) NOT NULL,
    CONSTRAINT FK_ImplementationCriteria_NonPriceElements FOREIGN KEY ([NonPriceElementsId]) REFERENCES [competitions].[NonPriceElements] ([Id]) ON DELETE CASCADE
)
