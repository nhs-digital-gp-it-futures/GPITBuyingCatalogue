CREATE TABLE [catalogue].[SolutionInProgressStandards]
(
    [SolutionId] NVARCHAR(14) NOT NULL,
    [StandardId] NVARCHAR(5) NOT NULL,
    CONSTRAINT FK_SolutionInProgressStandards_Solution FOREIGN KEY (SolutionId) REFERENCES catalogue.Solutions(CatalogueItemId) ON DELETE CASCADE,
    CONSTRAINT FK_SolutionInProgressStandards_Standard FOREIGN KEY (StandardId) REFERENCES catalogue.Standards(Id),
)
