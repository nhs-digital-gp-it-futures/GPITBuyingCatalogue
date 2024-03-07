CREATE TABLE [notifications].[ManagedEmailPreferences]
(
	[Id] INT NOT NULL,
    [Name] NVARCHAR(50) NOT NULL,
    [DefaultEnabled] BIT NOT NULL,
    CONSTRAINT PK_ManagedEmailPreferences PRIMARY KEY (Id),
)

-- 1 ContractExpiry
-- 2 PasswordExpiry
