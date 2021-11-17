CREATE TABLE catalogue.CapabilityCategories_History
(
     Id int NOT NULL,
     [Name] nvarchar(50) NOT NULL,
     [Description] nvarchar(200) NULL,
     LastUpdated datetime2(7) NOT NULL,
     LastUpdatedBy int NULL,
     SysStartTime datetime2(0) NOT NULL,
     SysEndTime datetime2(0) NOT NULL,
);
GO

CREATE CLUSTERED COLUMNSTORE INDEX IX_CapabilityCategories_History
ON catalogue.CapabilityCategories_History;
GO

CREATE NONCLUSTERED INDEX IX_CapabilitiyCategories_History_Id_Period_Columns
ON catalogue.CapabilityCategories_History (SysEndTime, SysStartTime, Id);
GO
