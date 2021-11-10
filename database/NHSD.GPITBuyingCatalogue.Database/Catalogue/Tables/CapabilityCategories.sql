CREATE TABLE catalogue.CapabilityCategories
(
     Id int NOT NULL,
     [Name] nvarchar(50) NOT NULL,
     [Description] nvarchar(200) NULL,
     SysStartTime datetime2(0) GENERATED ALWAYS AS ROW START NOT NULL,
     SysEndTime datetime2(0) GENERATED ALWAYS AS ROW END NOT NULL,
     PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime),     
     CONSTRAINT PK_CapabilityCategories PRIMARY KEY (Id),
     CONSTRAINT AK_CapabilityCategories_Name UNIQUE ([Name]),
);
