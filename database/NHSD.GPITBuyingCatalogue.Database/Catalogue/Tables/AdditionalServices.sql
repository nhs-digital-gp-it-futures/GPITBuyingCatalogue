CREATE TABLE catalogue.AdditionalServices
(
    CatalogueItemId nvarchar(14) NOT NULL,
    Summary nvarchar(300) NULL,
    FullDescription nvarchar(3000) NULL,
    LastUpdated datetime2(7) DEFAULT GETUTCDATE() NOT NULL,
    LastUpdatedBy int NULL,
    SolutionId nvarchar(14) NULL,
    SysStartTime datetime2(0) GENERATED ALWAYS AS ROW START NOT NULL,
    SysEndTime datetime2(0) GENERATED ALWAYS AS ROW END NOT NULL,
    PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime),
    CONSTRAINT PK_AdditionalServices PRIMARY KEY (CatalogueItemId),
    CONSTRAINT FK_AdditionalServices_CatalogueItem FOREIGN KEY (CatalogueItemId) REFERENCES catalogue.CatalogueItems(Id) ON DELETE CASCADE,
    CONSTRAINT FK_AdditionalServices_Solution FOREIGN KEY (SolutionId) REFERENCES catalogue.Solutions(CatalogueItemId),
    CONSTRAINT FK_AdditionalServices_LastUpdatedBy FOREIGN KEY (LastUpdatedBy) REFERENCES users.AspNetUsers(Id),
);
