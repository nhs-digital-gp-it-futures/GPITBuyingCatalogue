CREATE TABLE dbo.FrameworkCapabilities
(
     FrameworkId nvarchar(10) NOT NULL,
     CapabilityId uniqueidentifier NOT NULL,
     IsFoundation bit CONSTRAINT DF_FrameworkCapabilities_IsFoundation DEFAULT 0 NOT NULL,
     CONSTRAINT PK_FrameworkCapabilities PRIMARY KEY CLUSTERED (FrameworkId, CapabilityId),
     CONSTRAINT FK_FrameworkCapabilities_Capability FOREIGN KEY (CapabilityId) REFERENCES dbo.Capability(Id),
     CONSTRAINT FK_FrameworkCapabilities_Framework FOREIGN KEY (FrameworkId) REFERENCES dbo.Framework(Id)
);
