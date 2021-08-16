﻿CREATE TABLE catalogue.CapabilityCategories
(
     Id int NOT NULL,
     [Name] nvarchar(50) NOT NULL,
     CONSTRAINT PK_CapabilityCategories_Category PRIMARY KEY (Id),
     CONSTRAINT AK_CapabilityCategories_Name UNIQUE ([Name]),
);
