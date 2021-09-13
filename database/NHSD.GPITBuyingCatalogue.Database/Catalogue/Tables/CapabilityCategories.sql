CREATE TABLE catalogue.CapabilityCategories
(
     Id int NOT NULL,
     [Name] nvarchar(50) NOT NULL,
     [Description] nvarchar(200) NULL, 
     CONSTRAINT PK_CapabilityCategories PRIMARY KEY (Id),
     CONSTRAINT AK_CapabilityCategories_Name UNIQUE ([Name]),
);
