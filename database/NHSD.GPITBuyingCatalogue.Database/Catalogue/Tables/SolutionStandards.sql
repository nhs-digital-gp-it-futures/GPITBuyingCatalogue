CREATE TABLE [catalogue].[SolutionStandards]
(
    [SolutionId] NVARCHAR(14)       NOT NULL,
    [StandardId] NVARCHAR(5)        NOT NULL,
    [Status]     INT                NOT NULL,
    CONSTRAINT PK_SolutionStandards PRIMARY KEY ([SolutionId], [StandardId]),
    CONSTRAINT FK_SolutionStandards_Solution FOREIGN KEY ([SolutionId]) REFERENCES catalogue.Solutions ([CatalogueItemId]),
    CONSTRAINT FK_SolutionStandards_Standard FOREIGN KEY ([StandardId]) REFERENCES catalogue.Standards ([Id]),
    CONSTRAINT FK_SolutionStandards_Status FOREIGN KEY ([Status]) REFERENCES catalogue.SolutionStandardStatuses ([Id])
);
