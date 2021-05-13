CREATE TABLE dbo.SolutionCapability
(
     SolutionId nvarchar(14) NOT NULL,
     CapabilityId uniqueidentifier NOT NULL,
     StatusId int NOT NULL,
     LastUpdated datetime2(7) NOT NULL,
     LastUpdatedBy uniqueidentifier NOT NULL,
     CONSTRAINT PK_SolutionCapability PRIMARY KEY CLUSTERED (SolutionId, CapabilityId),
     CONSTRAINT FK_SolutionCapability_Capability FOREIGN KEY (CapabilityId) REFERENCES dbo.Capability(Id),
     CONSTRAINT FK_SolutionCapability_Solution FOREIGN KEY (SolutionId) REFERENCES dbo.Solution(Id) ON DELETE CASCADE,
     CONSTRAINT FK_SolutionCapability_SolutionCapabilityStatus FOREIGN KEY (StatusId) REFERENCES dbo.SolutionCapabilityStatus(Id)
);
