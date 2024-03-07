CREATE TABLE [notifications].[UserManagedEmailPreferences]
(
	[UserId] INT NOT NULL,
    [ManagedEmailPreferenceId] INT NOT NULL,
    [Enabled] BIT NOT NULL,
    CONSTRAINT PK_UserManagedEmailPreferences PRIMARY KEY (UserId, ManagedEmailPreferenceId),
    CONSTRAINT FK_UserManagedEmailPreferences_User FOREIGN KEY (UserId) REFERENCES users.AspNetUsers (Id),
    CONSTRAINT FK_UserManagedEmailPreferences_ManagedEmailPreferences FOREIGN KEY (ManagedEmailPreferenceId) REFERENCES notifications.ManagedEmailPreferences (Id),
)

-- 1 ContractExpiry
-- 2 PasswordExpiry
