CREATE TABLE filtering.ClientApplicationTypes
(
    Id int NOT NULL,
    [Name] nvarchar(35) NOT NULL,
    [Description] nvarchar(35) NOT NULL,
    CONSTRAINT PK_ClientApplicationType PRIMARY KEY (Id),
);
