CREATE TABLE catalogue.TimeUnits
(
    Id int NOT NULL,
    [Name] nvarchar(20) NOT NULL,
    [Description] nvarchar(32) NOT NULL,
    CONSTRAINT PK_TimeUnit PRIMARY KEY (Id),
);
