CREATE TABLE [ods_organisations].[Journal]
(
    [Id]            INT         NOT NULL PRIMARY KEY IDENTITY(1,1),
    [ImportDate]    DATETIME2   NOT NULL DEFAULT(GETUTCDATE())
)

GO
CREATE NONCLUSTERED INDEX IX_Journal_ImportDate ON [ods_organisations].[Journal] ([ImportDate])
