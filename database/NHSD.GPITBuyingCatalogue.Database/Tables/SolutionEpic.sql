CREATE TABLE dbo.SolutionEpic
(
     SolutionId nvarchar(14) NOT NULL,
     CapabilityId uniqueidentifier NOT NULL,
     EpicId nvarchar(10) NOT NULL,
     StatusId int NOT NULL,
     LastUpdated datetime2(7) NOT NULL,
     LastUpdatedBy uniqueidentifier NOT NULL,
     CONSTRAINT PK_SolutionEpic PRIMARY KEY CLUSTERED (SolutionId, CapabilityId, EpicId),
     CONSTRAINT FK_SolutionEpic_Capability FOREIGN KEY (CapabilityId) REFERENCES dbo.Capability(Id),
     CONSTRAINT FK_SolutionEpic_Epic FOREIGN KEY (EpicId) REFERENCES dbo.Epic(Id),
     CONSTRAINT FK_SolutionEpic_Solution FOREIGN KEY (SolutionId) REFERENCES dbo.Solution(Id) ON DELETE CASCADE,
     CONSTRAINT FK_SolutionEpic_SolutionEpicStatus FOREIGN KEY (StatusId) REFERENCES dbo.SolutionEpicStatus(Id),
     CONSTRAINT FK_SolutionEpicStatus FOREIGN KEY (StatusId) REFERENCES dbo.SolutionEpicStatus(Id)
);
