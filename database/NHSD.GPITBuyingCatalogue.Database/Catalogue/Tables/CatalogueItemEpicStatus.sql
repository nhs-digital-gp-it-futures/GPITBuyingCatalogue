CREATE TABLE catalogue.CatalogueItemEpicStatus
(
     Id int NOT NULL,
     [Name] nvarchar(16) NOT NULL,
     IsMet bit NOT NULL,
     SysStartTime datetime2(0) GENERATED ALWAYS AS ROW START NOT NULL,
     SysEndTime datetime2(0) GENERATED ALWAYS AS ROW END NOT NULL,
     PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime),
     CONSTRAINT PK_CatalogueItemEpicStatus PRIMARY KEY (Id),
     CONSTRAINT AK_CatalogueItemEpicStatus_Name UNIQUE ([Name]),
);
