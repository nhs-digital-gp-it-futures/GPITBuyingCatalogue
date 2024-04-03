CREATE TABLE [notifications].[EmailNotificationTypes]
(
	[Id] INT NOT NULL,
    [Name] NVARCHAR(20) NOT NULL,
    CONSTRAINT PK_EmailNotificationTypes PRIMARY KEY (Id),
)
