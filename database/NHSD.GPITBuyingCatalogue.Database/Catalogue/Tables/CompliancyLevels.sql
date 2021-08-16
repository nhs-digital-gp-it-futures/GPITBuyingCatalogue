CREATE TABLE catalogue.CompliancyLevels
(
     Id int NOT NULL,
     [Name] nvarchar(16) NOT NULL,
     CONSTRAINT PK_CompliancyLevels PRIMARY KEY (Id),
     CONSTRAINT AK_CompliancyLevels_Name UNIQUE ([Name]),
);
