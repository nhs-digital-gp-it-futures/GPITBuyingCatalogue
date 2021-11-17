CREATE TABLE catalogue.Epics_History
(
     Id nvarchar(10) NOT NULL,
     [Name] nvarchar(150) NOT NULL,
     CapabilityId int NOT NULL,
     SourceUrl nvarchar(max) NULL,
     CompliancyLevelId int NULL,
     IsActive bit NOT NULL,
     SupplierDefined bit NOT NULL,
     LastUpdated datetime2(7) NOT NULL,
     LastUpdatedBy int NULL,
     SysStartTime datetime2(0) NOT NULL,
     SysEndTime datetime2(0) NOT NULL
);
GO

CREATE CLUSTERED COLUMNSTORE INDEX IX_Epics_History
ON catalogue.Epics_History;
GO

CREATE NONCLUSTERED INDEX IX_Epics_History_Id_Period_Columns
ON catalogue.Epics_History (SysEndTime, SysStartTime, Id);
GO
