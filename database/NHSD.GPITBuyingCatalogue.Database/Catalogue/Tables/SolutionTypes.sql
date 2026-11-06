CREATE TABLE catalogue.SolutionTypes
(
    Id int NOT NULL,
    [Name] nvarchar(20) NOT NULL,
    CONSTRAINT PK_SolutionTypes PRIMARY KEY (Id),
    CONSTRAINT AK_SolutionTypes_Name UNIQUE ([Name]),
);
