CREATE TABLE catalogue.PublicationStatus
(
     Id int NOT NULL,
     [Name] nvarchar(16) NOT NULL,
     CONSTRAINT PK_PublicationStatus PRIMARY KEY (Id),
     CONSTRAINT AK_PublicationStatus_Name UNIQUE ([Name]),
);
