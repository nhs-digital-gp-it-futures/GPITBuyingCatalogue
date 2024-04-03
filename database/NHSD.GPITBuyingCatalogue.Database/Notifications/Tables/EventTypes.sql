CREATE TABLE [notifications].[EventTypes]
(
	[Id] INT NOT NULL,
    [Name] NVARCHAR(50) NOT NULL,
    [EmailPreferenceTypeId] INT NULL,
    CONSTRAINT PK_EventTypes PRIMARY KEY (Id),
    CONSTRAINT FK_EventTypes_EmailPreferenceType FOREIGN KEY (EmailPreferenceTypeId) REFERENCES notifications.EmailPreferenceTypes (Id),
)
