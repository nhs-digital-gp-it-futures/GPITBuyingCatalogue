CREATE TABLE catalogue.HostingTypes
(
    Id int NOT NULL,
    [Name] nvarchar(35) NOT NULL,
    [Description] nvarchar(35) NOT NULL,
    CONSTRAINT PK_HostingType PRIMARY KEY (Id),
);
