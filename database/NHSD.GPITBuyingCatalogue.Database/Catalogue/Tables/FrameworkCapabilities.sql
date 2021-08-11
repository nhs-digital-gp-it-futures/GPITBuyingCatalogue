CREATE TABLE catalogue.FrameworkCapabilities
(
     FrameworkId nvarchar(10) NOT NULL,
     CapabilityId int NOT NULL,
     IsFoundation bit CONSTRAINT DF_FrameworkCapabilities_IsFoundation DEFAULT 0 NOT NULL,
     CONSTRAINT PK_FrameworkCapabilities PRIMARY KEY CLUSTERED (FrameworkId, CapabilityId),
     CONSTRAINT FK_FrameworkCapabilities_Capability FOREIGN KEY (CapabilityId) REFERENCES catalogue.Capabilities(Id),
     CONSTRAINT FK_FrameworkCapabilities_Framework FOREIGN KEY (FrameworkId) REFERENCES catalogue.Frameworks(Id)
);
