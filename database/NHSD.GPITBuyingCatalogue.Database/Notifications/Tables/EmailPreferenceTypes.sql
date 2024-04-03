CREATE TABLE [notifications].[EmailPreferenceTypes]
(
	[Id] INT NOT NULL,
    [Name] NVARCHAR(50) NOT NULL,
    [DefaultEnabled] BIT NOT NULL,
    CONSTRAINT PK_EmailPreferenceTypes PRIMARY KEY (Id),
)
