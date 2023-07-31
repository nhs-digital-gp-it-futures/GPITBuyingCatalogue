CREATE TABLE [competitions].[NonPriceWeights]
(
    [NonPriceElementsId] INT NOT NULL PRIMARY KEY,
    [Implementation] INT NULL,
    [Interoperability] INT NULL,
    [ServiceLevel] INT NULL,
    CONSTRAINT FK_Weightings_NonPriceElements FOREIGN KEY ([NonPriceElementsId]) REFERENCES [competitions].[NonPriceElements] ([Id])
)
