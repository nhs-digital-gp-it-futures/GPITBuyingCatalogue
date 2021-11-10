CREATE TABLE catalogue.Frameworks
(
     Id nvarchar(10) NOT NULL,
     [Name] nvarchar(100) NOT NULL,
     ShortName nvarchar(25) NULL,
     [Description] nvarchar(max) NULL,
     [Owner] nvarchar(100) NULL,
     ActiveDate date NULL,
     ExpiryDate date NULL,
     SysStartTime datetime2(0) GENERATED ALWAYS AS ROW START NOT NULL,
     SysEndTime datetime2(0) GENERATED ALWAYS AS ROW END NOT NULL,
     PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime),
     CONSTRAINT PK_Frameworks PRIMARY KEY (Id),
);
