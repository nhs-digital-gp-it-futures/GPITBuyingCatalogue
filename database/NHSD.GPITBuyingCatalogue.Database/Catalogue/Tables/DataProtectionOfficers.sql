CREATE TABLE [catalogue].[DataProtectionOfficers]
(
    [Id]                    INT IDENTITY(1, 1)  NOT NULL PRIMARY KEY,
    [DataProcessingInfoId]  INT                 NOT NULL,
    [Name]                  NVARCHAR(200)       NOT NULL,
    [EmailAddress]          NVARCHAR(256)       NULL,
    [PhoneNumber]           NVARCHAR(35)        NULL,
    CONSTRAINT FK_DataProtectionOfficers_DataProcessingInformation FOREIGN KEY ([DataProcessingInfoId]) REFERENCES catalogue.DataProcessingInformation ([Id])
)
