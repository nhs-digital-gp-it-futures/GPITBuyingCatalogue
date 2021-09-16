CREATE TABLE catalogue.Epics
(
     Id nvarchar(10) NOT NULL,
     [Name] nvarchar(150) NOT NULL,
     CapabilityId int NOT NULL,
     SourceUrl nvarchar(max) NULL,
     CompliancyLevelId int NULL,
     IsActive bit NOT NULL DEFAULT 0,
     SupplierDefined bit CONSTRAINT DF_Epic_SupplierDefined DEFAULT 0 NOT NULL,
     CONSTRAINT PK_Epics PRIMARY KEY (Id),
     CONSTRAINT FK_Epics_Capability FOREIGN KEY (CapabilityId) REFERENCES catalogue.Capabilities(Id),
     CONSTRAINT FK_Epics_CompliancyLevel FOREIGN KEY (CompliancyLevelId) REFERENCES catalogue.CompliancyLevels(Id),
);
