CREATE TABLE catalogue.Capabilities_History
(
     Id int NOT NULL,
     CapabilityRef nvarchar(4),
     [Version] nvarchar(10) NULL,
     StatusId int NOT NULL,
     [Name] nvarchar(255) NOT NULL,
     [Description] nvarchar(500) NOT NULL,
     SourceUrl nvarchar(1000) NULL,
     EffectiveDate date NOT NULL,
     CategoryId int NOT NULL,
     LastUpdated datetime2(7) NOT NULL,
     LastUpdatedBy int NULL,
     SysStartTime datetime2(0) NOT NULL,
     SysEndTime datetime2(0) NOT NULL,
);
GO
