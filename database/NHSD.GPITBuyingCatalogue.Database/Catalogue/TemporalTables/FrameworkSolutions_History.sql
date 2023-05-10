CREATE TABLE catalogue.FrameworkSolutions_History
(
     FrameworkId nvarchar(36) NOT NULL,
     SolutionId nvarchar(14) NOT NULL,
     IsFoundation bit NOT NULL,
     LastUpdated datetime2(7)NOT NULL,
     LastUpdatedBy int NULL,
     SysStartTime datetime2(0) NOT NULL,
     SysEndTime datetime2(0) NOT NULL
);
