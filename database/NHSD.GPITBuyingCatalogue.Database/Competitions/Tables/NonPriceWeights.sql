﻿CREATE TABLE [competitions].[NonPriceWeights]
(
    [NonPriceElementsId] INT NOT NULL PRIMARY KEY,
    [Features] INT NULL,
    [Implementation] INT NULL,
    [Interoperability] INT NULL,
    [ServiceLevel] INT NULL,
    CONSTRAINT FK_Weightings_NonPriceElements FOREIGN KEY ([NonPriceElementsId]) REFERENCES [competitions].[NonPriceElements] ([Id]) ON DELETE CASCADE
)
