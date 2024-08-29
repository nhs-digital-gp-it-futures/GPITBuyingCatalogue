CREATE TABLE [catalogue].[DataProcessingInformation]
(
    [Id]                        INT IDENTITY(1, 1)  NOT NULL PRIMARY KEY,
    [SolutionId]                NVARCHAR(14)        NOT NULL,
    [DataProcessingDetailsId]   INT                 NOT NULL,
    CONSTRAINT FK_DataProcessingInformation_Solution FOREIGN KEY ([SolutionId]) REFERENCES catalogue.Solutions ([CatalogueItemId]),
    CONSTRAINT FK_DataProcessingInformation_DataProcessingDetails FOREIGN KEY ([DataProcessingDetailsId]) REFERENCES catalogue.DataProcessingDetails ([Id])
)
