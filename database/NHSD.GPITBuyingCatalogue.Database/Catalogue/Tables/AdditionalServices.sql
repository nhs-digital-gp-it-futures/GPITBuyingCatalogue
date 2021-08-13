﻿CREATE TABLE catalogue.AdditionalServices
(
    CatalogueItemId nvarchar(14) NOT NULL,
    Summary nvarchar(300) NULL,
    FullDescription nvarchar(3000) NULL,
    LastUpdated datetime2(7) NULL,
    LastUpdatedBy int NULL,
    SolutionId nvarchar(14) NULL,
    CONSTRAINT PK_AdditionalServices PRIMARY KEY (CatalogueItemId),
    CONSTRAINT FK_AdditionalServices_CatalogueItem FOREIGN KEY (CatalogueItemId) REFERENCES catalogue.CatalogueItems(Id) ON DELETE CASCADE,
    CONSTRAINT FK_AdditionalServices_Solution FOREIGN KEY (SolutionId) REFERENCES catalogue.Solutions(CatalogueItemId),
    CONSTRAINT FK_AdditionalServices_LastUpdatedBy FOREIGN KEY (LastUpdatedBy) REFERENCES users.AspNetUsers(Id),
);
