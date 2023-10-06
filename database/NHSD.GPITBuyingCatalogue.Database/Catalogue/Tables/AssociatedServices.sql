CREATE TABLE catalogue.AssociatedServices
(
    CatalogueItemId nvarchar(14) NOT NULL,
    [Description] nvarchar(1000) NULL,
    OrderGuidance nvarchar(1000) NULL,
    PracticeReorganisationType int NULL,
    LastUpdated datetime2(7) DEFAULT GETUTCDATE() NOT NULL,
    LastUpdatedBy int NULL,
    SysStartTime datetime2(0) GENERATED ALWAYS AS ROW START NOT NULL,
    SysEndTime datetime2(0) GENERATED ALWAYS AS ROW END NOT NULL,
    PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime),
    CONSTRAINT PK_AssociatedServices PRIMARY KEY (CatalogueItemId),
    CONSTRAINT FK_AssociatedServices_CatalogueItem FOREIGN KEY (CatalogueItemId) REFERENCES catalogue.CatalogueItems(Id) ON DELETE CASCADE,
    CONSTRAINT FK_AssociatedServices_LastUpdatedBy FOREIGN KEY (LastUpdatedBy) REFERENCES users.AspNetUsers(Id),
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = catalogue.AssociatedServices_History));
