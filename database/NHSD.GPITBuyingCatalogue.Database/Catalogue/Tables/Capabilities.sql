CREATE TABLE catalogue.Capabilities
(
     Id int NOT NULL,
     CapabilityRef AS 'C' + CAST(Id AS nvarchar(3)),
     [Version] nvarchar(10) NOT NULL,
     PreviousVersion nvarchar(10) NULL,
     StatusId int NOT NULL,
     [Name] nvarchar(255) NOT NULL,
     [Description] nvarchar(500) NOT NULL,
     SourceUrl nvarchar(1000) NULL,
     EffectiveDate date NOT NULL,
     CategoryId int NOT NULL,
     CONSTRAINT PK_Capabilities PRIMARY KEY (Id),
     CONSTRAINT FK_Capabilities_CapabilityCategory FOREIGN KEY (CategoryId) REFERENCES catalogue.CapabilityCategories(Id),
     CONSTRAINT FK_Capabilities_CapabilityStatus FOREIGN KEY (StatusId) REFERENCES catalogue.CapabilityStatus(Id),
     INDEX IX_Capabilities_CapabilityRef (CapabilityRef),
);
