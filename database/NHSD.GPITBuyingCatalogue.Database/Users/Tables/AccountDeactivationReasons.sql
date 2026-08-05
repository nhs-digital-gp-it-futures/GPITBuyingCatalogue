CREATE TABLE [users].[AccountDeactivationReasons]
(
    [Id] INT NOT NULL,
    [Reason] NVARCHAR(50) NOT NULL,
    CONSTRAINT PK_AccountDeactivationReasons PRIMARY KEY (Id),
)
