CREATE TABLE [catalogue].[DataProcessingDetails]
(
    [Id]                    INT IDENTITY(1, 1)  NOT NULL PRIMARY KEY,
    [Subject]               NVARCHAR(1500)      NOT NULL,
    [Duration]              NVARCHAR(1500)      NOT NULL,
    [ProcessingNature]      NVARCHAR(1500)      NOT NULL,
    [PersonalDataTypes]     NVARCHAR(1500)      NOT NULL,
    [DataSubjectCategories] NVARCHAR(1500)      NOT NULL,
)
