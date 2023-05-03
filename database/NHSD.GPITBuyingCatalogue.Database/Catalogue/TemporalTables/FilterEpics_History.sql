CREATE TABLE catalogue.FilterEpics_History
(
     FilterId nvarchar(10) NOT NULL,
     EpicId nvarchar(10) NOT NULL,
     LastUpdated datetime2(7) NOT NULL,
     LastUpdatedBy int NULL,
     SysStartTime datetime2(0) NOT NULL,
     SysEndTime datetime2(0) NOT NULL
);
