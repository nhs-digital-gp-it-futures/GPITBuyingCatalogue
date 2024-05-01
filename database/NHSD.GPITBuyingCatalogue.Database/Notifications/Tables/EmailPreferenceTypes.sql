CREATE TABLE [notifications].[EmailPreferenceTypes]
(
    [Id] INT NOT NULL,
    [Name] NVARCHAR(50) NOT NULL,
    [DefaultEnabled] BIT NOT NULL,
    [RoleType] INT DEFAULT(1) NOT NULL,
    CONSTRAINT PK_EmailPreferenceTypes PRIMARY KEY (Id),
    CONSTRAINT FK_EmailPreferenceTypes_RoleType FOREIGN KEY ([RoleType]) REFERENCES notifications.EmailPreferenceRoleTypes(Id)
)
