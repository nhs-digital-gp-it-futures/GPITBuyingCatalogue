CREATE TABLE [catalogue].[SolutionStandards]
(
    [Id]         INT IDENTITY (1,1) NOT NULL,
    [SolutionId] NVARCHAR(14)       NOT NULL,
    [StandardId] NVARCHAR(5)        NOT NULL,
    [Status]     INT                NOT NULL,
    CONSTRAINT PK_SolutionStandards PRIMARY KEY ([Id]),
    CONSTRAINT FK_SolutionStandards_Solution FOREIGN KEY ([SolutionId]) REFERENCES catalogue.Solutions ([CatalogueItemId]),
    CONSTRAINT FK_SolutionStandards_Standard FOREIGN KEY ([StandardId]) REFERENCES catalogue.Standards ([Id]),
    CONSTRAINT FK_SolutionStandards_Status FOREIGN KEY ([Status]) REFERENCES catalogue.SolutionStandardStatuses ([Id])
);
