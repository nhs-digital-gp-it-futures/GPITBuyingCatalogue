CREATE TABLE catalogue.AdditionalServices
(
    CatalogueItemId nvarchar(14) NOT NULL,
    Summary nvarchar(300) NULL,
    FullDescription nvarchar(3000) NULL,
    LastUpdated datetime2(7) NULL,
    LastUpdatedBy uniqueidentifier NULL,
    SolutionId nvarchar(14) NULL,
    CONSTRAINT PK_AdditionalService PRIMARY KEY (CatalogueItemId),
    CONSTRAINT FK_AdditionalService_CatalogueItem FOREIGN KEY (CatalogueItemId) REFERENCES catalogue.CatalogueItems(CatalogueItemId) ON DELETE CASCADE,
    CONSTRAINT FK_AdditionalService_Solution FOREIGN KEY (SolutionId) REFERENCES catalogue.Solutions(Id)
);
