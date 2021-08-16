CREATE TABLE catalogue.CapabilityStatus
(
     Id int NOT NULL,
     [Name] nvarchar(16) NOT NULL,
     CONSTRAINT PK_CapabilityStatus PRIMARY KEY (Id),
     CONSTRAINT AK_CapabilityStatus_Name UNIQUE ([Name]),
);
