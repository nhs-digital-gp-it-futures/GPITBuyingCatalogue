CREATE TABLE [notifications].[Events]
(
	[Id] INT NOT NULL,
    [Name] NVARCHAR(50) NOT NULL,
    [ManagedEmailPreferenceId] INT NULL,
    CONSTRAINT PK_Events PRIMARY KEY (Id),
    CONSTRAINT FK_Events_ManagedEmailPreference FOREIGN KEY (ManagedEmailPreferenceId) REFERENCES notifications.ManagedEmailPreferences (Id),
)

--        OrderCompleted = 1,                    null
--        OrderExpiryDueFirstThreshold = 2,      1
--        OrderExpiryDueSecondThreshold = 3,     1
--        UserPasswordExpired = 4                2

