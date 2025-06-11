CREATE TABLE [catalogue].[InProgressSolutionStandards]
(
    [SolutionId] NVARCHAR(14) NOT NULL,
    [StandardId] NVARCHAR(5) NOT NULL,
    CONSTRAINT PK_InProgressSolutionStandards PRIMARY KEY ([SolutionId], [StandardId]),
    CONSTRAINT FK_InProgressSolutionStandards_Solution FOREIGN KEY (SolutionId) REFERENCES catalogue.Solutions(CatalogueItemId) ON DELETE CASCADE,
    CONSTRAINT FK_InProgressSolutionStandards_Standard FOREIGN KEY (StandardId) REFERENCES catalogue.Standards(Id),
)
