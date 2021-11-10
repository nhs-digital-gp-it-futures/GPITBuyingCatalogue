﻿CREATE TABLE catalogue.PricingUnits
(
    Id smallint IDENTITY(1, 1) NOT NULL,
    [Name] nvarchar(20) NULL,
    TierName nvarchar(30) NULL,
    [Description] nvarchar(100) NOT NULL,
    [Definition] nvarchar(1000) NULL,
    SysStartTime datetime2(0) GENERATED ALWAYS AS ROW START NOT NULL,
    SysEndTime datetime2(0) GENERATED ALWAYS AS ROW END NOT NULL,
    PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime),
    CONSTRAINT PK_PricingUnits PRIMARY KEY (Id),
);
