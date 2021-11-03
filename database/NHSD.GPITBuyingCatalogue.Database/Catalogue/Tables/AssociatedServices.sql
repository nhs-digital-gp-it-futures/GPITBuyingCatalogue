CREATE TABLE catalogue.AssociatedServices
(
    CatalogueItemId nvarchar(14) NOT NULL,
    [Description] nvarchar(1000) NULL,
    OrderGuidance nvarchar(1000) NULL,
    LastUpdated datetime2(7) NOT NULL,
    LastUpdatedBy int NOT NULL,
    CONSTRAINT PK_AssociatedServices PRIMARY KEY (CatalogueItemId),
    CONSTRAINT FK_AssociatedServices_CatalogueItem FOREIGN KEY (CatalogueItemId) REFERENCES catalogue.CatalogueItems(Id) ON DELETE CASCADE,
    CONSTRAINT FK_AssociatedServices_LastUpdatedBy FOREIGN KEY (LastUpdatedBy) REFERENCES users.AspNetUsers(Id),
);
