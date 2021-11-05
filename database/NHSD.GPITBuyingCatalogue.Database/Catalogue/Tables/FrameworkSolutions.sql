CREATE TABLE catalogue.FrameworkSolutions
(
     FrameworkId nvarchar(10) NOT NULL,
     SolutionId nvarchar(14) NOT NULL,
     IsFoundation bit CONSTRAINT DF_FrameworkSolutions_IsFoundation DEFAULT 0 NOT NULL,
     LastUpdated datetime2(7) NOT NULL,
     LastUpdatedBy int NOT NULL,
     CONSTRAINT PK_FrameworkSolutions PRIMARY KEY (FrameworkId, SolutionId),
     CONSTRAINT FK_FrameworkSolutions_Framework FOREIGN KEY (FrameworkId) REFERENCES catalogue.Frameworks(Id),
     CONSTRAINT FK_FrameworkSolutions_Solution FOREIGN KEY (SolutionId) REFERENCES catalogue.Solutions(CatalogueItemId) ON DELETE CASCADE,
     CONSTRAINT FK_FrameworkSolutions_LastUpdatedBy FOREIGN KEY (LastUpdatedBy) REFERENCES users.AspNetUsers(Id),
);
