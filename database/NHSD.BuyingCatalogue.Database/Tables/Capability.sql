CREATE TABLE dbo.Capability
(
     Id uniqueidentifier NOT NULL,
     CapabilityRef nvarchar(10) NOT NULL,
     [Version] nvarchar(10) NOT NULL,
     PreviousVersion nvarchar(10) NULL,
     StatusId int NOT NULL,
     [Name] nvarchar(255) NOT NULL,
     [Description] nvarchar(500) NOT NULL,
     SourceUrl nvarchar(1000) NULL,
     EffectiveDate date NOT NULL,
     CategoryId int NOT NULL,
     CONSTRAINT PK_Capability PRIMARY KEY NONCLUSTERED (Id),
     CONSTRAINT FK_Capability_CapabilityCategory FOREIGN KEY (CategoryId) REFERENCES dbo.CapabilityCategory(Id),
     CONSTRAINT FK_Capability_CapabilityStatus FOREIGN KEY (StatusId) REFERENCES dbo.CapabilityStatus(Id),
     INDEX IX_CapabilityCapabilityRef CLUSTERED (CapabilityRef)
);
