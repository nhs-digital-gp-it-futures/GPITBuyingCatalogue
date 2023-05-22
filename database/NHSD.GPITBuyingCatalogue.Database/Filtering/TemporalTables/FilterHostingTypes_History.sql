CREATE TABLE filtering.FilterHostingTypes_History
(
     Id int NOT NULL,
     FilterId int NOT NULL,
     HostingTypeId int NOT NULL,
     LastUpdated datetime2(7) NOT NULL,
     LastUpdatedBy int NULL,
     SysStartTime datetime2(0) NOT NULL,
     SysEndTime datetime2(0) NOT NULL
);
