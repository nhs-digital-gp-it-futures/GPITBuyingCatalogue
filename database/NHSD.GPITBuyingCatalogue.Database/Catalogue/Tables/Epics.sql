CREATE TABLE catalogue.Epics
(
     Id nvarchar(10) NOT NULL,
     [Name] nvarchar(150) NOT NULL,
     CapabilityId uniqueidentifier NOT NULL,
     SourceUrl nvarchar(max) NULL,
     CompliancyLevelId int NULL,
     Active bit NOT NULL,
     SupplierDefined bit CONSTRAINT DF_Epic_SupplierDefined DEFAULT 0 NOT NULL,
     CONSTRAINT PK_Epic PRIMARY KEY NONCLUSTERED (Id),
     CONSTRAINT FK_Epic_Capability FOREIGN KEY (CapabilityId) REFERENCES catalogue.Capabilities(Id),
     CONSTRAINT FK_Epic_CompliancyLevel FOREIGN KEY (CompliancyLevelId) REFERENCES catalogue.CompliancyLevels(Id)
);
