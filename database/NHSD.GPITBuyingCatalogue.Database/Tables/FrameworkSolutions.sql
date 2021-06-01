CREATE TABLE dbo.FrameworkSolutions
(
     FrameworkId nvarchar(10) NOT NULL,
     SolutionId nvarchar(14) NOT NULL,
     IsFoundation bit CONSTRAINT DF_FrameworkSolutions_IsFoundation DEFAULT 0 NOT NULL,
     LastUpdated datetime2(7) NOT NULL,
     LastUpdatedBy uniqueidentifier NOT NULL,
     CONSTRAINT PK_FrameworkSolutions PRIMARY KEY CLUSTERED (FrameworkId, SolutionId),
     CONSTRAINT FK_FrameworkSolutions_Framework FOREIGN KEY (FrameworkId) REFERENCES dbo.Framework(Id),
     CONSTRAINT FK_FrameworkSolutions_Solution FOREIGN KEY (SolutionId) REFERENCES dbo.Solution(Id) ON DELETE CASCADE
);
