CREATE TABLE [notifications].[UserEmailPreferences]
(
	[UserId] INT NOT NULL,
    [EmailPreferenceTypeId] INT NOT NULL,
    [Enabled] BIT NOT NULL,
    CONSTRAINT PK_UserEmailPreferences PRIMARY KEY (UserId, EmailPreferenceTypeId),
    CONSTRAINT FK_UserEmailPreferences_User FOREIGN KEY (UserId) REFERENCES users.AspNetUsers (Id),
    CONSTRAINT FK_UserEmailPreferences_EmailPreferenceType FOREIGN KEY (EmailPreferenceTypeId) REFERENCES notifications.EmailPreferenceTypes (Id),
)
