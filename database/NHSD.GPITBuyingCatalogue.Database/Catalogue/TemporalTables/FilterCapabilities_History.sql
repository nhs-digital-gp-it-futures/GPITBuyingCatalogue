CREATE TABLE catalogue.FilterCapabilities_History
(
     FilterId int NOT NULL,
     CapabilityId int NOT NULL,
     LastUpdated datetime2(7)NOT NULL,
     LastUpdatedBy int NULL,
     SysStartTime datetime2(0) NOT NULL,
     SysEndTime datetime2(0) NOT NULL
);
