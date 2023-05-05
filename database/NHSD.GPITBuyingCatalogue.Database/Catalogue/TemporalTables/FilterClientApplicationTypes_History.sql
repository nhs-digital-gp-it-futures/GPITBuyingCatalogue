CREATE TABLE catalogue.FilterClientApplicationTypes_History
(
     FilterClientApplicationTypeId int NOT NULL,
     FilterId int NOT NULL,
     ClientApplicationTypeId int NOT NULL,
     LastUpdated datetime2(7) NOT NULL,
     LastUpdatedBy int NULL,
     SysStartTime datetime2(0) NOT NULL,
     SysEndTime datetime2(0) NOT NULL
);
