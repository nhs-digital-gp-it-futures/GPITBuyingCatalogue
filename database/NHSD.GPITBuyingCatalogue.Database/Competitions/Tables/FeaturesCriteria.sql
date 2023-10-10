CREATE TABLE [competitions].[FeaturesCriteria]
(
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [NonPriceElementsId] INT NOT NULL,
    [Requirements] NVARCHAR(1100) NOT NULL,
    [Compliance] INT NOT NULL,
    CONSTRAINT FK_FeaturesCriteria_NonPriceElements FOREIGN KEY ([NonPriceElementsId]) REFERENCES [competitions].[NonPriceElements] ([Id]) ON DELETE CASCADE,
    CONSTRAINT FK_FeaturesCriteria_Compliance FOREIGN KEY ([Compliance]) REFERENCES catalogue.CompliancyLevels  ([Id])
)
