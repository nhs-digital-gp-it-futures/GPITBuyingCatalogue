﻿CREATE TABLE catalogue.AssociatedServices_History
(
    CatalogueItemId nvarchar(14) NOT NULL,
    [Description] nvarchar(1000) NULL,
    OrderGuidance nvarchar(1000) NULL,
    LastUpdated datetime2(7) NOT NULL,
    LastUpdatedBy int NULL,
    SysStartTime datetime2(0) NOT NULL,
    SysEndTime datetime2(0) NOT NULL,
);
GO

CREATE CLUSTERED COLUMNSTORE INDEX IX_AssociatedServices_History
ON catalogue.AssociatedServices_History;
GO
