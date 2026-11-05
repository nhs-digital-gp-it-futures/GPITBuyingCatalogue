CREATE TABLE catalogue.SolutionSectionRequirements
(
     [Id] int NOT NULL,
     [Name] varchar(64) NOT NULL,
     CONSTRAINT PK_SolutionSectionRequirements PRIMARY KEY ([Id]),
     CONSTRAINT AK_SolutionSectionRequirements_Name UNIQUE ([Name]),
);